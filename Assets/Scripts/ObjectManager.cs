using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Object4D;

namespace Assets.Scripts
{
    static class ObjectManager
    {
        public static string GetShapeData(Shapes shape)
        {
            switch (shape)
            {
                case Shapes.Tesseract:
                    return TESSERACT_DATA;
                case Shapes.Cell_5:
                    return CELL_5_DATA;
            }
            return "";
        }
        static string TESSERACT_DATA { get => @"16
8

-0.5 -0.5 -0.5 -0.5
-0.5 -0.5 -0.5 0.5
-0.5 -0.5 0.5 -0.5
-0.5 -0.5 0.5 0.5
-0.5 0.5 -0.5 -0.5
-0.5 0.5 -0.5 0.5
-0.5 0.5 0.5 -0.5
-0.5 0.5 0.5 0.5
0.5 -0.5 -0.5 -0.5
0.5 -0.5 -0.5 0.5
0.5 -0.5 0.5 -0.5
0.5 -0.5 0.5 0.5
0.5 0.5 -0.5 -0.5
0.5 0.5 -0.5 0.5
0.5 0.5 0.5 -0.5
0.5 0.5 0.5 0.5

0
1
2
3
4
5
6
7

8
9
10
11
12
13
14
15

0
1
2
3
8
9
10
11

4
5
6
7
12
13
14
15

0
1
4
5
8
9
12
13

2
3
6
7
10
11
14
15

0
2
4
6
8
10
12
14

1
3
5
7
9
11
13
15
"; }
        static string CELL_5_DATA { get => @"5
5

1 1 1 -0.447
1 -1 -1 -0.447
-1 1 -1 -0.447
-1 -1 1 -0.447
0 0 0 1.788

0
1
2
3

0
1
2
4

0
1
3
4

0
2
3
4

1
2
3
4
"; }
    }
}
