using System;
using Raylib_cs;
using System.Numerics;

class Program
{
        public static float rx,ry,dx,dy,ra,fa;
        public static Vector2 rc;
        public static float RADA = 0.01745329f; // 1 degree in Radians
        public static Color rayColor;

        // initialize
        public const int WINHEIGHT = 512;
        public const int WINWIDTH = 1024;

        public static void DrawRays(float playerAngle, int cellSize, Vector2 playerPos, int[][] map, int mapWidth)
        {
            ra = playerAngle - (RADA * 30); // set the ray angle to the player angle - 30
            
            // final angle
            fa = 0;

            // capping ray angle
            if (ra < 0) ra += 2 * MathF.PI;
            if(ra > 360) ra -= 2 * MathF.PI;

            // ray variables

            // the ray position is on the right corner of the player
            rx = playerPos.X + 5; 
            ry = playerPos.Y + 5;

            // the step the rays will increase by every frame
            dx = MathF.Cos(ra);
            dy = MathF.Sin(ra);
            
            // vector for ray position (optional)
            rc = new();

            // ray color for shading and stuff
            rayColor = Color.Blue;

            // for loop
            for (int i = 0; i < 120; i++)
            {
                // reset each ray
                dx = MathF.Cos(ra);
                dy = MathF.Sin(ra);
                rx = playerPos.X + 5;
                ry = playerPos.Y + 5;
                rayColor = new (255,255,255);

                while(true)
                {
                    // add by dx and dy until it hits a wall
                    rx += dx; 
                    ry += dy;

                    // if the ray hits a wall
                    if (map[(int)ry/cellSize][(int)rx/cellSize] == 1)
                    {
                        ra += RADA/2; // increase the ray angle
                        rc = new(rx, ry); // change the ray coordinate
                        break; // break out of the loop
                    }
                }

                // stop the fisheye effect
                fa = playerAngle - ra; // the fa is the actual angle of the singular ray
                float rDist = DistanceV(new(playerPos.X + 5, playerPos.Y + 5), rc); // distance from the ray and the player

                // set it white if its really close
                rayColor = new(228-(int)(rDist/5),0,124-(int)(rDist/25));
                if ((int)(rDist/5) <= 5) {rayColor = new(228,0,124);}

                rDist *= MathF.Cos(fa); // find the vertical length of it using cosine
                // on the screen
                float lineH = mapWidth*320 / rDist; // line height is a number divided by distance to make perscpective
                float lineO = 320 - (lineH/2); // line offset
                Raylib.DrawLineEx(new((i*9),lineO), new((i*9), (lineH+lineO)), 9f, rayColor); // draw the actual ray
                Raylib.DrawLineEx(new((i*9), (lineH+lineO)), new((i*9), WINHEIGHT), 9f, Color.White); // floor

            }
        }
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
        public static void Main()
        {
            

            Raylib.InitWindow(WINWIDTH, WINHEIGHT, "Maze Game");

            Raylib.SetTargetFPS(60);

            // map variables
            int mapX = 16;
            int mapY = 8;
            int mapWidth = 64;
            int[][] map = [
                [1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
                [1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],
                [1,0,1,1,1,1,1,1,0,1,0,0,1,0,0,1],
                [1,0,0,0,1,0,0,1,0,1,1,1,1,1,0,1],
                [1,0,1,1,1,0,0,1,0,0,1,0,0,1,0,1],
                [1,0,1,0,0,0,1,1,1,1,1,0,1,1,0,1],
                [1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1],
                [1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]
            ];

             // player
            Vector2 playerPos = new(mapX * mapWidth / 2, mapY * mapWidth / 2);
            float playerAngle = 0;
            float pdx = 0f;
            float pdy = 0f;

            while(!Raylib.WindowShouldClose())
            {
                // player movement
                if (Raylib.IsKeyDown(KeyboardKey.A))
                {
                    playerAngle -= 0.02f; // decrease angle
                    if (playerAngle < 0) {playerAngle += 2*MathF.PI;} // cap the angle
                    pdx = MathF.Cos(playerAngle) * 1.5f; // change pdx to move in direction
                    pdy = MathF.Sin(playerAngle) * 1.5f; // change pdy to move in direction
                    
                }
                if (Raylib.IsKeyDown(KeyboardKey.D))
                {
                    playerAngle += 0.02f; // increase angle
                    if (playerAngle > 2*MathF.PI) {playerAngle -= 2*MathF.PI;} // cap the angle
                    pdx = MathF.Cos(playerAngle) * 1.5f; // change pdx to move in direction
                    pdy = MathF.Sin(playerAngle) * 1.5f; // change pdy to move in direction
                    
                }
            
                if (Raylib.IsKeyDown(KeyboardKey.W)) {playerPos.X += pdx; playerPos.Y += pdy;}
                // if you hit a wall reverse the movement KEY.W
                if (map[(int)playerPos.Y/mapWidth][(int)(playerPos.X+(MathF.Cos(playerAngle)*10))/mapWidth] == 1){playerPos.X -= 2*pdx; playerPos.Y -= 2*pdy;}


                if (Raylib.IsKeyDown(KeyboardKey.S)) {playerPos.X -= pdx; playerPos.Y -= pdy;}
                // if you hit a wall reverse the movement KEY.S
                if (map[(int)playerPos.Y/mapWidth][(int)(playerPos.X+(MathF.Cos(-playerAngle)*10))/mapWidth] == 1){playerPos.X += 2*pdx; playerPos.Y += 2*pdy;}



                // start drawing
                Raylib.BeginDrawing();

                // background
                Raylib.ClearBackground(Color.SkyBlue);

                // Rays
                DrawRays(playerAngle, mapWidth, playerPos, map, mapWidth); // Rays

                // end drawing
                Raylib.EndDrawing();
            }

            // Unloading
            Raylib.CloseWindow();
        }        
}
