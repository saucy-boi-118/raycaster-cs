using System;
using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;



namespace RaycastGame
{
    class Program
    {
        public static float DistanceV(Vector2 A, Vector2 B)
        {
        // find the distance between two vectors on the X and Z plane
        
        return (float)(Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2)));
        }
        public static void DrawMap(int[][] mapArray, int mapWidth, int mapY, int mapX)
        {
            for (int y = 0; y < mapY; y++)
            {
                for (int x = 0; x < mapX; x++)
                {
                    if (mapArray[y][x] == 1)
                    {
                        Raylib.DrawRectangle(x * mapWidth, y * mapWidth, mapWidth, mapWidth, Color.White);
                    } 
                    else
                    {
                        Raylib.DrawRectangleLines(x * mapWidth, y * mapWidth, mapWidth, mapWidth, Color.White);
                    }
                }
            }
        }

        public static void DrawRays(int fov, float playerAngle, int cellSize, Vector2 playerPos, int[][] map, int mapWidth)
        {         
            // angles stuff
            float radA = 0.01745329f; // 1 deg in radians
            float ra = playerAngle - (radA * 30);
            float fa = 0f;
            if (ra < 0) ra += 2 * MathF.PI;
            if(ra > 360) ra -= 2 * MathF.PI;
            //lines
            float lineH = 0f;
            float lineO = 0f;
            float rDist = 0f;
            // positions
            float rx = playerPos.X + 5;
            float ry = playerPos.Y + 5;
            float dx = MathF.Cos(ra);
            float dy = MathF.Sin(ra);
            Vector2 rc = new();
            // for loop
            for (int r = 0; r < fov; r++)
            {
                // reset each ray
                dx = MathF.Cos(ra);
                dy = MathF.Sin(ra);
                rx = playerPos.X + 5;
                ry = playerPos.Y + 5;
                while(true)
                {
                    // add by the COSINE and SINE everytime theres no wall
                    rx += dx; 
                    ry += dy;
                    if (map[(int)ry/cellSize][(int)rx/cellSize] == 1)
                    {
                        ra += radA;
                        if (ra > 2*MathF.PI){ra -= 2*MathF.PI;} if (ra < 0){ra += 2*MathF.PI;}
                        rc = new(rx, ry);                        
                        break;
                    }
                }
                Raylib.DrawLineEx(new(playerPos.X +5, playerPos.Y + 5), rc, 1.5f, Color.Red);
                // stop the fisheye effect
                fa = playerAngle - ra;
                if (fa > 2*MathF.PI){fa -= 2*MathF.PI;} if (fa < 0){fa += 2*MathF.PI;}
                rDist = DistanceV(new(playerPos.X + 5, playerPos.Y + 5), rc);
                rDist *= MathF.Cos(fa);
                // on the screen
                lineH = mapWidth*320 / DistanceV(new(playerPos.X + 5, playerPos.Y + 5), rc);
                lineO = 320 - (lineH/2);
                Raylib.DrawLineEx(new((r*8)+530,lineO), new((r*8)+530, lineH+lineO), 8f, Color.Blue);


            }
        }
        public static void Main()
        {
            // init
            const int WINHEIGHT = 512;
            const int WINWIDTH = 1024;

            Raylib.InitWindow(WINWIDTH, WINHEIGHT, "Raycaster");

            Raylib.SetTargetFPS(60);
            

            // Variables

            // Map
            int mapX = 8;
            int mapY = 8;
            int mapWidth = 64;
            int[][] map = [
                [1,1,1,1,1,1,1,1],
                [1,0,1,0,0,0,0,1],
                [1,0,1,1,0,0,0,1],
                [1,0,1,0,0,0,0,1],
                [1,0,0,0,1,1,0,1],
                [1,0,0,0,0,1,0,1],
                [1,0,0,0,0,1,0,1],
                [1,1,1,1,1,1,1,1]
            ];

            // player
            Vector2 playerPos = new(mapX * mapWidth / 2, mapY * mapWidth / 2);
            float playerAngle = 0;
            float pdx = 0f;
            float pdy = 0f;



            // Loop
            while(!Raylib.WindowShouldClose())
            {
                // before draw
                if (Raylib.IsKeyDown(KeyboardKey.A))
                {
                    playerPos.X -= 3;
                    playerAngle -= 0.1f;
                    if (playerAngle < 0) {playerAngle += 2*MathF.PI;}
                    pdx = MathF.Cos(playerAngle) * 5;
                    pdy = MathF.Sin(playerAngle) * 5;
                    
                }
                if (Raylib.IsKeyDown(KeyboardKey.D))
                {
                    playerPos.X += 3;
                    playerAngle += 0.1f;
                    if (playerAngle > 2*MathF.PI) {playerAngle -= 2*MathF.PI;}
                    pdx = MathF.Cos(playerAngle) * 5;
                    pdy = MathF.Sin(playerAngle) * 5;
                    
                }
                if (Raylib.IsKeyDown(KeyboardKey.W)) {playerPos.X += pdx; playerPos.Y += pdy;}
                if (Raylib.IsKeyDown(KeyboardKey.S)) {playerPos.X -= pdx; playerPos.Y -= pdy;}



                // draw
                Raylib.BeginDrawing();

                Raylib.ClearBackground(Color.Gray);


                // drawing the map
                DrawMap(map, mapWidth, mapY, mapX);
                
                // draw rays
                DrawRays(60, playerAngle, mapWidth, playerPos, map, mapWidth);

                //player
                Raylib.DrawRectangleV(playerPos, new(10f, 10f), Color.Yellow);

                // draw lines from player
                Raylib.DrawLineEx(new(playerPos.X + 5, playerPos.Y + 5), //line start
                new((playerPos.X + 5) + pdx * 5, (playerPos.Y + 5) + pdy * 5), //line end
                3f, Color.Yellow); // color and width


                
                

                Raylib.EndDrawing();
            }


            // unload / close
            Raylib.CloseWindow();
        }
    }
}