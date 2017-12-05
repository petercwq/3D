using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace magic_cube {
    public class Cube2D {
        private int size;
        public CubeFace[,] projection{get; private set;}
        
        public Cube2D(int size) {
            this.size = size;
            this.projection = new CubeFace[size*4,size*3];
            createCube();
        }

        public Cube2D(int size, CubeFace[,] c) {
            this.size = size;
            this.projection = c;

            if (!isValidProjection()) {
                throw new InvalidDataException();
            }
        }

        private bool isValidProjection() {
            Dictionary<CubeFace, int> ct = new Dictionary<CubeFace, int>{
                {CubeFace.L, 0},
                {CubeFace.D, 0},
                {CubeFace.R, 0},
                {CubeFace.B, 0},
                {CubeFace.F, 0},
                {CubeFace.U, 0},
                {CubeFace.None, 0},
            };

            for (int i = 0; i < size * 4; i++) {
                for (int j = 0; j < size * 3; j++) {
                    ct[projection[i, j]]++;
                }
            }

            foreach (var f in ct) {
                if (f.Value != 9 && f.Key != CubeFace.None) {
                    return false;
                }
            }

            return true;
        }

        private void createCube(){
            for(int i=0; i<size*4; i++){
                for(int j=0; j<size*3; j++){
                    if (i < size && j >= size && j < size * 2) {
                        projection[i, j] = CubeFace.B;
                    }
                    else if (i >= size && i < size * 2) {
                        if (j < size) {
                            projection[i, j] = CubeFace.L;
                        }
                        else if (j >= size && j < size * 2) {
                            projection[i, j] = CubeFace.D;
                        }
                        else {
                            projection[i, j] = CubeFace.R;
                        }
                    }
                    else if (i >= size * 2 && i < size * 3 && j >= size && j < size * 2) {
                        projection[i, j] = CubeFace.F;
                    }
                    else if (i >= size * 3 && j >= size && j < size * 2) {
                        projection[i, j] = CubeFace.U;
                    }
                    else {
                        projection[i, j] = CubeFace.None;
                    }
                }
            }
        }

        public void rotate(KeyValuePair<Move, RotationDirection> move) {
            switch (move.Key) {
                case Move.F:
                case Move.S:
                    rotateFS(move.Key, move.Value);
                    break;
                case Move.B:
                    rotateB(move.Value);
                    break;
                case Move.R:
                    rotateR(move.Value);
                    break;
                case Move.L:
                case Move.M:
                    rotateLM(move.Key, move.Value);
                    break;
                case Move.U:
                    rotateU(move.Value);
                    break;
                case Move.D:
                    rotateD(move.Value);
                    break;
                case Move.E:
                    rotateE(move.Value);
                    break;
            }
        }

        public bool isUnscrambled() {
            return isUnscrambledB() && isUnscrambledD() && isUnscrambledF() && isUnscrambledL() && isUnscrambledR() && isUnscrambledU();
        }

        private bool isUnscrambledB() {
            CubeFace f = projection[0, size];

            for (int i = 0; i < size; i++ ) {
                for (int j = size; j < size * 2; j++) {
                    if(projection[i,j] != f){
                        return false;
                    }
                }
            }

            return true;
        }

        private bool isUnscrambledD() {
            CubeFace f = projection[size, size];

            for (int i = size; i < size*2; i++) {
                for (int j = size; j < size * 2; j++) {
                    if (projection[i, j] != f) {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool isUnscrambledF() {
            CubeFace f = projection[size*2, size];

            for (int i = size*2; i < size*3; i++) {
                for (int j = size; j < size * 2; j++) {
                    if (projection[i, j] != f) {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool isUnscrambledU() {
            CubeFace f = projection[size*3, size];

            for (int i = size*3; i < size*4; i++) {
                for (int j = size; j < size * 2; j++) {
                    if (projection[i, j] != f) {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool isUnscrambledL() {
            CubeFace f = projection[size, 0];

            for (int i = size; i < size*2; i++) {
                for (int j = 0; j < size; j++) {
                    if (projection[i, j] != f) {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool isUnscrambledR() {
            CubeFace f = projection[size, size*2];
            for (int i = size; i < size * 2; i++) {
                for (int j = size*2; j < size*3; j++) {
                    if (projection[i, j] != f) {
                        return false;
                    }
                }
            }

            return true;
        }

        private void rotateR(RotationDirection d) {
            CubeFace t;
            int j = 5;

            List<List<int>> substitutions = new List<List<int>> {
                new List<int>{3, 8, 5, 8},
                new List<int>{5, 8, 5, 6},
                new List<int>{5, 6, 3, 6},
                new List<int>{3, 6, 3, 8},
                new List<int>{5, 7, 4, 6},
                new List<int>{4, 6, 3, 7},
                new List<int>{3, 7, 4, 8},
                new List<int>{4, 8, 5, 7},
            };

            if (d == RotationDirection.ClockWise) {
                t = projection[0, j];
                projection[0, j] = projection[9, j];
                projection[9, j] = projection[6, j];
                projection[6, j] = projection[3, j];
                projection[3, j] = t;

                t = projection[1, j];
                projection[1, j] = projection[10, j];
                projection[10, j] = projection[7, j];
                projection[7, j] = projection[4, j];
                projection[4, j] = t;

                t = projection[2, j];
                projection[2, j] = projection[11, j];
                projection[11, j] = projection[8, j];
                projection[8, j] = projection[5, j];
                projection[5, j] = t;
            }
            else {
                t = projection[9, j];
                projection[9, j] = projection[0, j];
                projection[0, j] = projection[3, j];
                projection[3, j] = projection[6, j];
                projection[6, j] = t;

                t = projection[4, j];
                projection[4, j] = projection[7, j];
                projection[7, j] = projection[10, j];
                projection[10, j] = projection[1, j];
                projection[1, j] = t;

                t = projection[11, j];
                projection[11, j] = projection[2, j];
                projection[2, j] = projection[5, j];
                projection[5, j] = projection[8, j];
                projection[8, j] = t;
            }

            rotateFace(substitutions, d);
        }

        private void rotateLM(Move m, RotationDirection d) {
            CubeFace t;
            int j;

            List<List<int>> substitutions = new List<List<int>> {
                new List<int>{5, 0, 3, 0},
                new List<int>{5, 2, 5, 0},
                new List<int>{3, 2, 5, 2},
                new List<int>{3, 0, 3, 2},
                new List<int>{4, 2, 5, 1},
                new List<int>{3, 1, 4, 2},
                new List<int>{4, 0, 3, 1},
                new List<int>{5, 1, 4, 0},
            };

            if (m == Move.L) {
                j = 3;
            }
            else {
                j = 4;
            }

            if(d == RotationDirection.ClockWise){
                t = projection[9, j];
                projection[9, j] = projection[0, j];
                projection[0, j] = projection[3, j];
                projection[3, j] = projection[6, j];
                projection[6, j] = t;

                t = projection[4, j];
                projection[4, j] = projection[7, j];
                projection[7, j] = projection[10, j];
                projection[10, j] = projection[1, j];
                projection[1, j] = t;

                t = projection[11, j];
                projection[11, j] = projection[2, j];
                projection[2, j] = projection[5, j];
                projection[5, j] = projection[8, j];
                projection[8, j] = t;

            }
            else{
                t = projection[0, j];
                projection[0, j] = projection[9, j];
                projection[9, j] = projection[6, j];
                projection[6, j] = projection[3, j];
                projection[3, j] = t;

                t = projection[1, j];
                projection[1, j] = projection[10, j];
                projection[10, j] = projection[7, j];
                projection[7, j] = projection[4, j];
                projection[4, j] = t;

                t = projection[2, j];
                projection[2, j] = projection[11, j];
                projection[11, j] = projection[8, j];
                projection[8, j] = projection[5, j];
                projection[5, j] = t;
            }

            if (m == Move.L) {
                rotateFace(substitutions, d);
            }
        }

        private void rotateFS(Move m, RotationDirection d) {
            CubeFace t;
            int i1 = 5, i2 = 9;

            List<List<int>> substitutions = new List<List<int>> {
                new List<int>{6, 3, 6, 5},
                new List<int>{6, 5, 8, 5},
                new List<int>{8, 5, 8, 3},
                new List<int>{8, 3, 6, 3},
                new List<int>{6, 4, 7, 5},
                new List<int>{7, 5, 8, 4},
                new List<int>{8, 4, 7, 3},
                new List<int>{7, 3, 6, 4},
            };

            if (m == Move.S) {
                i1 = 4;
                i2 = 10;
            }

            if (d == RotationDirection.ClockWise) {
                t = projection[i1, 5];
                projection[i1, 5] = projection[i1, 8];
                projection[i1, 8] = projection[i2, 3];
                projection[i2, 3] = projection[i1, 2];
                projection[i1, 2] = t;

                t = projection[i1, 4];
                projection[i1, 4] = projection[i1, 7];
                projection[i1, 7] = projection[i2, 4];
                projection[i2, 4] = projection[i1, 1];
                projection[i1, 1] = t;

                t = projection[i1, 3];
                projection[i1, 3] = projection[i1, 6];
                projection[i1, 6] = projection[i2, 5];
                projection[i2, 5] = projection[i1, 0];
                projection[i1, 0] = t;
            }
            else {
                t = projection[i1, 8];
                projection[i1, 8] = projection[i1, 5];
                projection[i1, 5] = projection[i1, 2];
                projection[i1, 2] = projection[i2, 3];
                projection[i2, 3] = t;

                t = projection[i1, 7];
                projection[i1, 7] = projection[i1, 4];
                projection[i1, 4] = projection[i1, 1];
                projection[i1, 1] = projection[i2, 4];
                projection[i2, 4] = t;

                t = projection[i1, 6];
                projection[i1, 6] = projection[i1, 3];
                projection[i1, 3] = projection[i1, 0];
                projection[i1, 0] = projection[i2, 5];
                projection[i2, 5] = t;
            }

            if(m == Move.F){
                rotateFace(substitutions, d);
            }
        }

        private void rotateB(RotationDirection d) {
            CubeFace t;

            List<List<int>> substitutions = new List<List<int>> {
                new List<int>{0, 3, 0, 5},
                new List<int>{0, 5, 2, 5},
                new List<int>{2, 5, 2, 3},
                new List<int>{2, 3, 0, 3},
                new List<int>{0, 4, 1, 5},
                new List<int>{1, 5, 2, 4},
                new List<int>{2, 4, 1, 3},
                new List<int>{1, 3, 0, 4},
            };

            if (d == RotationDirection.ClockWise) {
                t = projection[3, 3];
                projection[3, 3] = projection[3, 0];
                projection[3, 0] = projection[11, 5];
                projection[11, 5] = projection[3, 6];
                projection[3, 6] = t;

                t = projection[11, 3];
                projection[11, 3] = projection[3, 8];
                projection[3, 8] = projection[3, 5];
                projection[3, 5] = projection[3, 2];
                projection[3, 2] = t;

                t = projection[3, 4];
                projection[3, 4] = projection[3, 1];
                projection[3, 1] = projection[11, 4];
                projection[11, 4] = projection[3, 7];
                projection[3, 7] = t;
            }
            else {
                t = projection[3, 0];
                projection[3, 0] = projection[3, 3];
                projection[3, 3] = projection[3, 6];
                projection[3, 6] = projection[11, 5];
                projection[11, 5] = t;

                t = projection[3, 8];
                projection[3, 8] = projection[11, 3];
                projection[11, 3] = projection[3, 2];
                projection[3, 2] = projection[3, 5];
                projection[3, 5] = t;

                t = projection[3, 1];
                projection[3, 1] = projection[3, 4];
                projection[3, 4] = projection[3, 7];
                projection[3, 7] = projection[11, 4];
                projection[11, 4] = t;
            }

            rotateFace(substitutions, d);
        }

        private void rotateD(RotationDirection d) {
            CubeFace t;

            List<List<int>> substitutions = new List<List<int>> {
                new List<int>{3, 3, 3, 5},
                new List<int>{3, 5, 5, 5},
                new List<int>{5, 5, 5, 3},
                new List<int>{5, 3, 3, 3},
                new List<int>{3, 4, 4, 5},
                new List<int>{4, 5, 5, 4},
                new List<int>{5, 4, 4, 3},
                new List<int>{4, 3, 3, 4},
            };

            if (d == RotationDirection.ClockWise) {
                t = projection[3, 2];
                projection[3, 2] = projection[2, 5];
                projection[2, 5] = projection[5, 6];
                projection[5, 6] = projection[6, 3];
                projection[6, 3] = t;

                t = projection[4, 2];
                projection[4, 2] = projection[2, 4];
                projection[2, 4] = projection[4, 6];
                projection[4, 6] = projection[6, 4];
                projection[6, 4] = t;

                t = projection[5, 2];
                projection[5, 2] = projection[2, 3];
                projection[2, 3] = projection[3, 6];
                projection[3, 6] = projection[6, 5];
                projection[6, 5] = t;
            }
            else {
                t = projection[2, 5];
                projection[2, 5] = projection[3, 2];
                projection[3, 2] = projection[6, 3];
                projection[6, 3] = projection[5, 6];
                projection[5, 6] = t;

                t = projection[2, 4];
                projection[2, 4] = projection[4, 2];
                projection[4, 2] = projection[6, 4];
                projection[6, 4] = projection[4, 6];
                projection[4, 6] = t;

                t = projection[5, 2];
                projection[5, 2] = projection[6, 5];
                projection[6, 5] = projection[3, 6];
                projection[3, 6] = projection[2, 3];
                projection[2, 3] = t;
            }

            rotateFace(substitutions, d);
        }

        private void rotateE(RotationDirection d) {
            CubeFace t;

            if (d == RotationDirection.ClockWise) {
                t = projection[3, 1];
                projection[3, 1] = projection[1, 5];
                projection[1, 5] = projection[5, 7];
                projection[5, 7] = projection[7, 3];
                projection[7, 3] = t;

                t = projection[4, 1];
                projection[4, 1] = projection[1, 4];
                projection[1, 4] = projection[4, 7];
                projection[4, 7] = projection[7, 4];
                projection[7, 4] = t;

                t = projection[5, 1];
                projection[5, 1] = projection[1, 3];
                projection[1, 3] = projection[3, 7];
                projection[3, 7] = projection[7, 5];
                projection[7, 5] = t;
            }
            else {
                t = projection[1, 5];
                projection[1, 5] = projection[3, 1];
                projection[3, 1] = projection[7, 3];
                projection[7, 3] = projection[5, 7];
                projection[5, 7] = t;

                t = projection[1, 4];
                projection[1, 4] = projection[4, 1];
                projection[4, 1] = projection[7, 4];
                projection[7, 4] = projection[4, 7];
                projection[4, 7] = t;

                t = projection[5, 1];
                projection[5, 1] = projection[7, 5];
                projection[7, 5] = projection[3, 7];
                projection[3, 7] = projection[1, 3];
                projection[1, 3] = t;
            }
        }

        private void rotateU(RotationDirection d) {
            CubeFace t;

            List<List<int>> substitutions = new List<List<int>> {
                new List<int>{9, 3, 9, 5},
                new List<int>{9, 5, 11, 5},
                new List<int>{11, 5, 11, 3},
                new List<int>{11, 3, 9, 3},
                new List<int>{9, 4, 10, 5},
                new List<int>{10, 5, 11, 4},
                new List<int>{11, 4, 10, 3},
                new List<int>{10, 3, 9, 4},
            };

            if (d == RotationDirection.ClockWise) {
                t = projection[3, 0];
                projection[3, 0] = projection[8, 3];
                projection[8, 3] = projection[5, 8];
                projection[5, 8] = projection[0, 5];
                projection[0, 5] = t;

                t = projection[4, 0];
                projection[4, 0] = projection[8, 4];
                projection[8, 4] = projection[4, 8];
                projection[4, 8] = projection[0, 4];
                projection[0, 4] = t;

                t = projection[5, 0];
                projection[5, 0] = projection[8, 5];
                projection[8, 5] = projection[3, 8];
                projection[3, 8] = projection[0, 3];
                projection[0, 3] = t;
            }
            else {
                t = projection[3, 0];
                projection[3, 0] = projection[0, 5];
                projection[0, 5] = projection[5, 8];
                projection[5, 8] = projection[8, 3];
                projection[8, 3] = t;

                t = projection[4, 0];
                projection[4, 0] = projection[0, 4];
                projection[0, 4] = projection[4, 8];
                projection[4, 8] = projection[8, 4];
                projection[8, 4] = t;

                t = projection[5, 0];
                projection[5, 0] = projection[0, 3];
                projection[0, 3] = projection[3, 8];
                projection[3, 8] = projection[8, 5];
                projection[8, 5] = t;
            }

            rotateFace(substitutions, d);
        }

        private void rotateFace(List<List<int>> substitutions, RotationDirection d) {
            CubeFace[,] current = (CubeFace[,])projection.Clone();

            int first_lhs = 0, second_lhs = 1, first_rhs = 2, second_rhs = 3;

            if (d == RotationDirection.CounterClockWise) {
                first_lhs = 2;
                second_lhs = 3;

                first_rhs = 0;
                second_rhs = 1;
            }

            foreach(List<int> s in substitutions){
                projection[s[first_lhs], s[second_lhs]] = current[s[first_rhs], s[second_rhs]];
            }
        }

        public void dbg(){
            for (int i = 0; i < size * 4; i++) {
                for (int j = 0; j < size * 3; j++) {
                    Debug.Write(projection[i, j].ToString().PadLeft(5, ' '));
                }
                Debug.WriteLine("");
            }
        }
    }
}
