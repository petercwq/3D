using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace magic_cube {
    class InputOutput {
        private int size;

        public InputOutput(int s) {
            this.size = s;
        }

        public void save(string fileName, CubeFace[,] projection, List<KeyValuePair<Move, RotationDirection>> moves) {
            using (StreamWriter f = new StreamWriter(fileName)) {
                for (int i = 0; i < size * 4; i++) {
                    for (int j = 0; j < size * 3; j++) {
                        f.Write(projection[i, j].ToString() + " ");
                    }
                    f.WriteLine();
                }

                foreach (var m in moves) {
                    f.WriteLine("{0} {1}", m.Key, m.Value);
                }
            }
        }

        public CubeFace[,] read(string fileName, out List<KeyValuePair<Move, RotationDirection>> moves) {
            CubeFace[,] projection = new CubeFace[size * 4, size * 3];

            using (StreamReader r = new StreamReader(fileName)) {
                for (int i = 0; i < size * 4; i++) {
                    string[] line = null;
                    try {
                        line = r.ReadLine().Split(' ');
                    }
                    catch (NullReferenceException) {
                        throw new InvalidDataException();
                    }
                    for (int j = 0; j < size * 3; j++) {
                        try {
                            projection[i, j] = (CubeFace)Enum.Parse(typeof(CubeFace), line[j]);
                        }
                        catch (ArgumentException) {
                            throw new InvalidDataException();
                        }
                        catch (IndexOutOfRangeException) {
                            throw new InvalidDataException();
                        }
                    }
                }

                moves = new List<KeyValuePair<Move, RotationDirection>>();

                while (!r.EndOfStream) {
                    string[] line = null;
                    line = r.ReadLine().Split(' ');

                    try {
                        moves.Add(new KeyValuePair<Move, RotationDirection>((Move)Enum.Parse(typeof(Move), line[0]),
                            (RotationDirection)Enum.Parse(typeof(RotationDirection), line[1])));
                    }
                    catch (ArgumentException) {
                        throw new InvalidDataException();
                    }
                }
            }

            return projection;
        }

    }
}
