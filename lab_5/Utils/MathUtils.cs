namespace Utils
{
    public static class MathUtils
    {
        public static T[] InitArray<T>(int size, T value)
        {
            var matrix = new T[size];
            for (int i = 0; i < size; i++)
                    matrix[i] = value;
            return matrix;
        }

        public static T[,] InitMatrix<T>(int rows, int cols, T value)
        {
            var matrix = new T[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = value;
            return matrix;
        }
    }
}