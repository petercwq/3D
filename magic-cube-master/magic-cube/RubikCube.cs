using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Windows;
using System.Threading;
using System.IO;

namespace magic_cube {
    public class RubikCube : Cube {
        /// <summary>
        /// The cube will be size x size x size
        /// </summary>
        private int size;

        private Point3D origin;

        /// <summary>
        /// Length of the cube edge
        /// </summary>
        private double edge_len;

        /// <summary>
        /// Space between the cubes forming the bigger cube
        /// </summary>
        private double space;

        public Cube2D projection;
        public TimeSpan animationDuration;

        private List<KeyValuePair<Move, RotationDirection>> moves;
        int index;
        bool animation_lock = false;

        private Dictionary<CubeFace, Material> faceColors = new Dictionary<CubeFace, Material> {
            {CubeFace.L, new DiffuseMaterial(new SolidColorBrush(Colors.Red))},
            {CubeFace.D, new DiffuseMaterial(new SolidColorBrush(Colors.Yellow))},
            {CubeFace.B, new DiffuseMaterial(new SolidColorBrush(Colors.Green))},
            {CubeFace.R, new DiffuseMaterial(new SolidColorBrush(Colors.Orange))},
            {CubeFace.U, new DiffuseMaterial(new SolidColorBrush(Colors.White))},
            {CubeFace.F, new DiffuseMaterial(new SolidColorBrush(Colors.Blue))}
        };
        
        public RubikCube(int size, Point3D o, TimeSpan duration, double len = 1, double space = 0.1) {
            this.size = size;
            this.origin = o;
            this.edge_len = len;
            this.space = space;
            this.projection = new Cube2D(size);
            this.animationDuration = duration;

            createCube();
        }

        public RubikCube(CubeFace[,] projection, int size, Point3D o, TimeSpan duration, double len = 1, double space = 0.1) {
            this.size = size;
            this.origin = o;
            this.edge_len = len;
            this.space = space;
            this.projection = new Cube2D(size, projection);
            this.animationDuration = duration;

            createCubeFromProjection();
        }

        private void createCubeFromProjection() {
            Cube c;
            Dictionary<CubeFace, Material> colors;

            double x_offset, y_offset, z_offset;

            for (int y = 0; y < size; y++) {
                for (int z = 0; z < size; z++) {
                    for (int x = 0; x < size; x++) {
                        if (y == 1 && x == 1 && z == 1) {
                            continue;
                        }

                        x_offset = (edge_len + space) * x;
                        y_offset = (edge_len + space) * y;
                        z_offset = (edge_len + space) * z;

                        Point3D p = new Point3D(origin.X + x_offset, origin.Y + y_offset, origin.Z + z_offset);

                        colors = setFaceColorsFromProjection(x, y, z, projection.projection);

                        c = new Cube(p, edge_len, colors, getPossibleMoves(x, y, z));
                        this.Children.Add(c);
                    }
                }
            }
        }

        protected override void createCube() {
            Cube c;
            Dictionary<CubeFace, Material> colors;

            double x_offset, y_offset, z_offset;

            for (int y = 0; y < size; y++) {
                for (int z = 0; z < size; z++) {
                    for (int x = 0; x < size; x++) {
                         if (y == 1 && x == 1 && z == 1) {
                             continue;
                        }

                        x_offset = (edge_len + space) * x;
                        y_offset = (edge_len + space) * y;
                        z_offset = (edge_len + space) * z;

                        Point3D p = new Point3D(origin.X + x_offset, origin.Y + y_offset, origin.Z + z_offset);

                        colors = setFaceColors(x, y, z);

                        c = new Cube(p, edge_len, colors, getPossibleMoves(x, y, z));
                        this.Children.Add(c);
                    }
                }
            }
        }

        private HashSet<Move> getPossibleMoves(int x, int y, int z){
            HashSet<Move> moves =  new HashSet<Move>();

            if (y == 0) {
                moves.Add(Move.D);
            }
            else if (y == size - 1) {
                moves.Add(Move.U);
            }
            else {
                moves.Add(Move.E);
            }

            if (x == 0) {
                moves.Add(Move.L);
            }
            else if (x == size - 1) {
                moves.Add(Move.R);
            }
            else {
                moves.Add(Move.M);
            }

            if (z == 0) {
                moves.Add(Move.B);
            }
            else if (z == size - 1) {
                moves.Add(Move.F);
            }
            else {
                moves.Add(Move.S);
            }

            return moves;
        }

        public void rotate(List<KeyValuePair<Move, RotationDirection>> moves) {
            if (animation_lock) {
                return;
            }

            animation_lock = true;
            index = 0;
            this.moves = moves;

            animate(index);
            index++;
        }

        void animation_Completed(object sender, EventArgs e) {
            if (index < moves.Count) {
                animate(index);
                index++;
            }
            else {
                animation_lock = false;
            }
        }

        void animate(int i) {
            Dictionary<Move, CubeFace> dominantFaces = new Dictionary<Move, CubeFace> {
                {Move.B, CubeFace.R},
                {Move.D, CubeFace.R},
                {Move.E, CubeFace.R},
                {Move.F, CubeFace.R},
                {Move.L, CubeFace.F},
                {Move.M, CubeFace.F},
                {Move.R, CubeFace.F},
                {Move.S, CubeFace.R},
                {Move.U, CubeFace.F},
            };

            HashSet<Move> possibleMoves = new HashSet<Move>();
            Vector3D axis = new Vector3D();
            double angle = 90 * Convert.ToInt32(moves[i].Value);
            axis = getRotationAxis(moves[i].Key);

            AxisAngleRotation3D rotation = new AxisAngleRotation3D(axis, angle);
            RotateTransform3D transform = new RotateTransform3D(rotation, new Point3D(0, 0, 0));

            DoubleAnimation animation = new DoubleAnimation(0, angle, animationDuration);

            foreach (Cube c in this.Children) {
                possibleMoves = new HashSet<Move>(c.possibleMoves);
                possibleMoves.Remove((Move)dominantFaces[moves[i].Key]);

                if (possibleMoves.Contains(moves[i].Key)) {
                    c.possibleMoves = getNextPossibleMoves(c.possibleMoves, moves[i].Key, moves[i].Value);
                    c.rotations.Children.Add(transform); rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
                }
            }

            animation.Completed += new EventHandler(animation_Completed);
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
            projection.rotate(moves[i]);
        }

        public bool rotate(KeyValuePair<Move, RotationDirection> move) {
            if (animation_lock) {
                return false;
            }

            Dictionary<Move, CubeFace> dominantFaces = new Dictionary<Move, CubeFace> {
                {Move.B, CubeFace.R},
                {Move.D, CubeFace.R},
                {Move.E, CubeFace.R},
                {Move.F, CubeFace.R},
                {Move.L, CubeFace.F},
                {Move.M, CubeFace.F},
                {Move.R, CubeFace.F},
                {Move.S, CubeFace.R},
                {Move.U, CubeFace.F},
            };

            HashSet<Move> possibleMoves = new HashSet<Move>();
            Vector3D axis = getRotationAxis(move.Key);
            
            double angle = 90 * Convert.ToInt32(move.Value);

            AxisAngleRotation3D rotation = new AxisAngleRotation3D(axis, angle);
            RotateTransform3D transform = new RotateTransform3D(rotation, new Point3D(0, 0, 0));

            DoubleAnimation animation = new DoubleAnimation(0, angle, animationDuration);
            
            foreach(Cube c in this.Children){
                possibleMoves = new HashSet<Move>(c.possibleMoves);
                possibleMoves.Remove((Move)dominantFaces[move.Key]);
                if(possibleMoves.Contains(move.Key)){
                    c.possibleMoves = getNextPossibleMoves(c.possibleMoves, move.Key, move.Value);

                    c.rotations.Children.Add(transform);
                }
            }

            projection.rotate(move);
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);

            return true;
        }

        private Vector3D getRotationAxis(Move m){
            Vector3D axis = new Vector3D();

             switch (m) {
                case Move.F:
                case Move.S:
                    axis.X = 0;
                    axis.Y = 0;
                    axis.Z = -1;
                    break;
                case Move.R:
                    axis.X = -1;
                    axis.Y = 0;
                    axis.Z = 0;
                    break;
                case Move.B:
                    axis.X = 0;
                    axis.Y = 0;
                    axis.Z = 1;
                    break;
                case Move.L:
                case Move.M:
                    axis.X = 1;
                    axis.Y = 0;
                    axis.Z = 0;
                    break;
                case Move.U:
                    axis.X = 0;
                    axis.Y = -1;
                    axis.Z = 0;
                    break;
                case Move.D:
                case Move.E:
                    axis.X = 0;
                    axis.Y = 1;
                    axis.Z = 0;
                    break;
            }

            return axis;
        }

        private HashSet<Move> getNextPossibleMoves(HashSet<Move>moves, Move m, RotationDirection direction){
            //HashSet<Move> iMoves = new HashSet<Move>(moves);
             Dictionary<Move, List<List<Move>>> substitutions = new Dictionary<Move, List<List<Move>>> {
                {Move.F, new List<List<Move>>{
                    new List<Move>{Move.U, Move.L, Move.U, Move.R},
                    new List<Move>{Move.U, Move.R, Move.D, Move.R},
                    new List<Move>{Move.D, Move.R, Move.D, Move.L},
                    new List<Move>{Move.D, Move.L, Move.U, Move.L},
                    new List<Move>{Move.U, Move.M, Move.E, Move.R},
                    new List<Move>{Move.E, Move.R, Move.M, Move.D},
                    new List<Move>{Move.M, Move.D, Move.L, Move.E},
                    new List<Move>{Move.L, Move.E, Move.U, Move.M},
                }},
                {Move.B, new List<List<Move>>{
                    new List<Move>{Move.R, Move.U, Move.L, Move.U},
                    new List<Move>{Move.R, Move.D, Move.R, Move.U},
                    new List<Move>{Move.L, Move.D, Move.R, Move.D},
                    new List<Move>{Move.L, Move.U, Move.L, Move.D},
                    new List<Move>{Move.R, Move.E, Move.M, Move.U},
                    new List<Move>{Move.D, Move.M, Move.R, Move.E},
                    new List<Move>{Move.E, Move.L, Move.D, Move.M},
                    new List<Move>{Move.M, Move.U, Move.E, Move.L},
                }},
                {Move.U, new List<List<Move>>{
                    new List<Move>{Move.B, Move.L, Move.B, Move.R},
                    new List<Move>{Move.B, Move.R, Move.F, Move.R},
                    new List<Move>{Move.F, Move.R, Move.F, Move.L},
                    new List<Move>{Move.F, Move.L, Move.B, Move.L},
                    new List<Move>{Move.B, Move.M, Move.S, Move.R},
                    new List<Move>{Move.S, Move.R, Move.M, Move.F},
                    new List<Move>{Move.M, Move.F, Move.L, Move.S},
                    new List<Move>{Move.L, Move.S, Move.B, Move.M},
                }},                
                {Move.D, new List<List<Move>>{
                    new List<Move>{Move.R, Move.B, Move.L, Move.B},
                    new List<Move>{Move.R, Move.F, Move.R, Move.B},
                    new List<Move>{Move.L, Move.F, Move.R, Move.F},
                    new List<Move>{Move.L, Move.B, Move.L, Move.F},
                    new List<Move>{Move.R, Move.S, Move.M, Move.B},
                    new List<Move>{Move.F, Move.M, Move.R, Move.S},
                    new List<Move>{Move.S, Move.L, Move.F, Move.M},
                    new List<Move>{Move.M, Move.B, Move.S, Move.L},
                }},
                {Move.L, new List<List<Move>>{
                    new List<Move>{Move.B, Move.U, Move.F, Move.U},
                    new List<Move>{Move.F, Move.U, Move.F, Move.D},
                    new List<Move>{Move.F, Move.D, Move.B, Move.D},
                    new List<Move>{Move.B, Move.D, Move.B, Move.U},
                    new List<Move>{Move.S, Move.U, Move.E, Move.F},
                    new List<Move>{Move.E, Move.F, Move.D, Move.S},
                    new List<Move>{Move.D, Move.S, Move.B, Move.E},
                    new List<Move>{Move.B, Move.E, Move.S, Move.U},
                }},  
                {Move.R, new List<List<Move>>{
                    new List<Move>{Move.U, Move.F, Move.U, Move.B},
                    new List<Move>{Move.D, Move.F, Move.U, Move.F},
                    new List<Move>{Move.D, Move.B, Move.D, Move.F},
                    new List<Move>{Move.U, Move.B, Move.D, Move.B},
                    new List<Move>{Move.F, Move.E, Move.U, Move.S},
                    new List<Move>{Move.S, Move.D, Move.F, Move.E},
                    new List<Move>{Move.E, Move.B, Move.S, Move.D},
                    new List<Move>{Move.U, Move.S, Move.E, Move.B},
                }},                
                {Move.M, new List<List<Move>>{
                    new List<Move>{Move.U, Move.F, Move.D, Move.F},
                    new List<Move>{Move.D, Move.F, Move.D, Move.B},
                    new List<Move>{Move.B, Move.D, Move.B, Move.U},
                    new List<Move>{Move.B, Move.U, Move.F, Move.U},
                    new List<Move>{Move.E, Move.F, Move.D, Move.S},
                    new List<Move>{Move.D, Move.S, Move.B, Move.E},
                    new List<Move>{Move.B, Move.E, Move.U, Move.S},
                    new List<Move>{Move.U, Move.S, Move.E, Move.F},
                }},         
                {Move.E, new List<List<Move>>{
                    new List<Move>{Move.L, Move.F, Move.R, Move.F},
                    new List<Move>{Move.R, Move.F, Move.R, Move.B},
                    new List<Move>{Move.R, Move.B, Move.L, Move.B},
                    new List<Move>{Move.L, Move.B, Move.L, Move.F},
                    new List<Move>{Move.M, Move.F, Move.R, Move.S},
                    new List<Move>{Move.R, Move.S, Move.B, Move.M},
                    new List<Move>{Move.B, Move.M, Move.L, Move.S},
                    new List<Move>{Move.L, Move.S, Move.M, Move.F},
                }},         
                {Move.S, new List<List<Move>>{
                    new List<Move>{Move.U, Move.R, Move.D, Move.R},
                    new List<Move>{Move.D, Move.R, Move.D, Move.L},
                    new List<Move>{Move.D, Move.L, Move.U, Move.L},
                    new List<Move>{Move.U, Move.L, Move.U, Move.R},
                    new List<Move>{Move.M, Move.U, Move.E, Move.R},
                    new List<Move>{Move.E, Move.R, Move.M, Move.D},
                    new List<Move>{Move.M, Move.D, Move.E, Move.L},
                    new List<Move>{Move.E, Move.L, Move.M, Move.U},
                }},
            };

            foreach (List<Move> s in substitutions[m]) {
                if (direction == RotationDirection.ClockWise) {
                    if (moves.Contains(s[0]) && moves.Contains(s[1])) {
                        moves.Remove(s[0]);
                        moves.Add(s[2]);

                        moves.Remove(s[1]);
                        moves.Add(s[3]);

                        break;
                    }
                }
                else {
                    if (moves.Contains(s[2]) && moves.Contains(s[3])) {
                        moves.Remove(s[2]);
                        moves.Add(s[0]);

                        moves.Remove(s[3]);
                        moves.Add(s[1]);

                        break;
                    }
                }
            }

            /*
            if (moves.Count != 3) {
                if(true){}
            }
            */
            return moves;
        }

        public bool isUnscrambled() {
            return projection.isUnscrambled();
        }

        private Dictionary<CubeFace, Material> setFaceColors(int x, int y, int z){
            Dictionary<CubeFace, Material> colors = new Dictionary<CubeFace,Material>();

            if (x == 0) {
                colors.Add(CubeFace.L, faceColors[CubeFace.L]);
            }

            if (y == 0) {
                colors.Add(CubeFace.D, faceColors[CubeFace.D]);
            }

            if (z == 0) {
                colors.Add(CubeFace.B, faceColors[CubeFace.B]);
            }

            if (x == size-1) {
                colors.Add(CubeFace.R, faceColors[CubeFace.R]);
            }

            if (y == size - 1) {
                colors.Add(CubeFace.U, faceColors[CubeFace.U]);
            }

            if (z == size - 1) {
                colors.Add(CubeFace.F, faceColors[CubeFace.F]);
            }

            return colors;
        }

        private Dictionary<CubeFace, Material> setFaceColorsFromProjection(int x, int y, int z, CubeFace[,] p) {
            Dictionary<Tuple<int, int, int>, Dictionary<CubeFace, Material>> colors = new Dictionary<Tuple<int, int, int>, Dictionary<CubeFace, Material>> {
                {new Tuple<int, int, int>(0, 0, 0), new Dictionary<CubeFace, Material>{
                    {CubeFace.L, faceColors[p[3,2]]},
                    {CubeFace.B, faceColors[p[2,3]]},
                    {CubeFace.D, faceColors[p[3,3]]},
                }},
                {new Tuple<int, int, int>(1, 0, 0), new Dictionary<CubeFace, Material>{
                    {CubeFace.D, faceColors[p[3,4]]},
                    {CubeFace.B, faceColors[p[2,4]]},
                }},
                {new Tuple<int, int, int>(2, 0, 0), new Dictionary<CubeFace, Material>{
                    {CubeFace.R, faceColors[p[3,6]]},
                    {CubeFace.B, faceColors[p[2,5]]},
                    {CubeFace.D, faceColors[p[3,5]]},
                }},

                {new Tuple<int, int, int>(0, 0, 1), new Dictionary<CubeFace, Material>{
                    {CubeFace.L, faceColors[p[4,2]]},
                    {CubeFace.D, faceColors[p[4,3]]},
                }},
                {new Tuple<int, int, int>(1, 0, 1), new Dictionary<CubeFace, Material>{
                    {CubeFace.D, faceColors[p[4,4]]},
                }},
                {new Tuple<int, int, int>(2, 0, 1), new Dictionary<CubeFace, Material>{
                    {CubeFace.R, faceColors[p[4,6]]},
                    {CubeFace.D, faceColors[p[4,5]]},
                }},

                {new Tuple<int, int, int>(0, 0, 2), new Dictionary<CubeFace, Material>{
                    {CubeFace.L, faceColors[p[5,2]]},
                    {CubeFace.F, faceColors[p[6,3]]},
                    {CubeFace.D, faceColors[p[5,3]]},
                }},
                {new Tuple<int, int, int>(1, 0, 2), new Dictionary<CubeFace, Material>{
                    {CubeFace.F, faceColors[p[6,4]]},
                    {CubeFace.D, faceColors[p[5,4]]},
                }},
                {new Tuple<int, int, int>(2, 0, 2), new Dictionary<CubeFace, Material>{
                    {CubeFace.R, faceColors[p[5,6]]},
                    {CubeFace.F, faceColors[p[6,5]]},
                    {CubeFace.D, faceColors[p[5,5]]},
                }},

                {new Tuple<int, int, int>(0, 1, 0), new Dictionary<CubeFace, Material>{
                    {CubeFace.L, faceColors[p[3,1]]},
                    {CubeFace.B, faceColors[p[1,3]]},
                }},
                {new Tuple<int, int, int>(1, 1, 0), new Dictionary<CubeFace, Material>{
                    {CubeFace.B, faceColors[p[1,4]]},
                }},
                {new Tuple<int, int, int>(2, 1, 0), new Dictionary<CubeFace, Material>{
                    {CubeFace.R, faceColors[p[3,7]]},
                    {CubeFace.B, faceColors[p[1,5]]},
                }},
                
                {new Tuple<int, int, int>(0, 1, 1), new Dictionary<CubeFace, Material>{
                    {CubeFace.L, faceColors[p[4,1]]},
                }},/*
                {new Tuple<int, int, int>(1, 1, 1), new Dictionary<CubeFace, Material>{
                    {CubeFace.B, faceColors[p[1,4]]}, //empty because we are in the middle of the cube!
                }},*/
                {new Tuple<int, int, int>(2, 1, 1), new Dictionary<CubeFace, Material>{
                    {CubeFace.R, faceColors[p[4,7]]},
                }},
                                
                {new Tuple<int, int, int>(0, 1, 2), new Dictionary<CubeFace, Material>{
                    {CubeFace.L, faceColors[p[5,1]]},
                    {CubeFace.F, faceColors[p[7,3]]},
                }},
                {new Tuple<int, int, int>(1, 1, 2), new Dictionary<CubeFace, Material>{
                    {CubeFace.F, faceColors[p[7,4]]},
                }},
                {new Tuple<int, int, int>(2, 1, 2), new Dictionary<CubeFace, Material>{
                    {CubeFace.R, faceColors[p[5,7]]},
                    {CubeFace.F, faceColors[p[7,5]]},
                }},

                {new Tuple<int, int, int>(0, 2, 0), new Dictionary<CubeFace, Material>{
                    {CubeFace.L, faceColors[p[3,0]]},
                    {CubeFace.B, faceColors[p[0,3]]},
                    {CubeFace.U, faceColors[p[11,3]]},
                }},
                {new Tuple<int, int, int>(1, 2, 0), new Dictionary<CubeFace, Material>{
                    {CubeFace.U, faceColors[p[11,4]]},
                    {CubeFace.B, faceColors[p[0,4]]},
                }},
                {new Tuple<int, int, int>(2, 2, 0), new Dictionary<CubeFace, Material>{
                    {CubeFace.R, faceColors[p[3,8]]},
                    {CubeFace.B, faceColors[p[0,5]]},
                    {CubeFace.U, faceColors[p[11,5]]},
                }},

                {new Tuple<int, int, int>(0, 2, 1), new Dictionary<CubeFace, Material>{
                    {CubeFace.L, faceColors[p[4,0]]},
                    {CubeFace.U, faceColors[p[10,3]]},
                }},
                {new Tuple<int, int, int>(1, 2, 1), new Dictionary<CubeFace, Material>{
                    {CubeFace.U, faceColors[p[10,4]]},
                }},
                {new Tuple<int, int, int>(2, 2, 1), new Dictionary<CubeFace, Material>{
                    {CubeFace.R, faceColors[p[4,8]]},
                    {CubeFace.U, faceColors[p[10,5]]},
                }},

                {new Tuple<int, int, int>(0, 2, 2), new Dictionary<CubeFace, Material>{
                    {CubeFace.L, faceColors[p[5,0]]},
                    {CubeFace.F, faceColors[p[8,3]]},
                    {CubeFace.U, faceColors[p[9,3]]},
                }},
                {new Tuple<int, int, int>(1, 2, 2), new Dictionary<CubeFace, Material>{
                    {CubeFace.U, faceColors[p[9,4]]},
                    {CubeFace.F, faceColors[p[8,4]]},
                }},
                {new Tuple<int, int, int>(2, 2, 2), new Dictionary<CubeFace, Material>{
                    {CubeFace.R, faceColors[p[5,8]]},
                    {CubeFace.F, faceColors[p[8,5]]},
                    {CubeFace.U, faceColors[p[9,5]]},
                }},
            };

            return colors[new Tuple<int, int, int>(x, y, z)];
        }
    }
}
