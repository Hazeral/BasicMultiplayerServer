using System;

namespace MultiplayerTestServer
{
    class Map
    {
        public static float[,] Bounds =
        {
            { -8, 8 }, // X
            { -4, 4 }  // Y
        };

        public static float[] RandomCoordinates()
        {
            Random r = new Random();
            int x = r.Next((int)Bounds[0, 0], (int)Bounds[0, 1]);
            int y = r.Next((int)Bounds[1, 0], (int)Bounds[1, 1]);

            return new float[] { x, y };
        }
    }
}
