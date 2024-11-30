using System;

namespace AES_First_Round
{
    public class AES
    {
        private static readonly byte[] _sBlocks = { 0x63, 0xCA, 0xF0, 0xF0, 0xA2, 0xF2, 0x59, 0xAF, 0xAB, 0xA4, 0x76, 0x72, 0xA4, 0xFA, 0x7B, 0x33 };

        public static void Main(string[] args)
        {
            string myWord = "ADCRIPTOGRAPHYAT";
            string masterKey = "ATTESTATIONNUMB2";
            Console.WriteLine($"Исходное слово - {myWord}\nКлюч - {masterKey}");
            byte[] myWordBytes = System.Text.Encoding.UTF8.GetBytes(myWord);
            Console.WriteLine("Моя строка в HEX представлении");
            foreach (byte b in myWordBytes)
            {
                Console.Write(b.ToString("X2") + " ");
            }
            Console.WriteLine();
            byte[] myKeyBytes = System.Text.Encoding.UTF8.GetBytes(masterKey);

            Console.WriteLine("Мой ключ в HEX представлении");
            foreach (byte b in myKeyBytes)
            {
                Console.Write(b.ToString("X2") + " ");
            }
            Console.WriteLine();

            // XOR
            byte[] xor = Xor(myWordBytes, myKeyBytes);

            Console.WriteLine("Результат после Xor:");
            foreach (byte b in xor)
            {
                Console.Write(b.ToString("X2") + " ");
            }
            Console.WriteLine();

            Console.WriteLine("Результат после подстановки:");
            byte[] Sbox = SBox(xor);
            foreach (byte b in Sbox)
            {
                Console.Write(b.ToString("X2") + " ");
            }
            Console.WriteLine();
            Console.WriteLine();

            // Преобразование в матрицу
            byte[,] state = ConvertToMatrix(Sbox);

            Console.WriteLine("Подстановка в виде матрицы:");
            PrintMatrix(state);

            Console.WriteLine();
            Console.WriteLine("Подстановка после ShiftRows:");
            // ShiftRows
            ShiftRows(state);
            PrintMatrix(state);
            Console.WriteLine();

            byte[,] multiuplyBy2 = MultiplyMatrixBy2(state);
            Console.WriteLine("NixColumns * 2:");
            PrintMatrix(multiuplyBy2);

            byte[] multiuplyBy3 = Xor(FlattenMatrix(multiuplyBy2), FlattenMatrix(state));

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("NixColumns * 3:");
            byte[,] conM = ConvertToMatrix(multiuplyBy3);
            PrintMatrix(conM);
            Console.WriteLine();

            byte[] flattenCon3 = FlattenMatrix(conM);

            byte[] finalResult = Xor(flattenCon3, myKeyBytes);

            byte[,] finalRestoMatrix = ConvertToMatrix(finalResult);
            Console.WriteLine("AddRoundKey:");
            PrintMatrix(finalRestoMatrix);







        }

        static byte[] FlattenMatrix(byte[,] matrix)
        {
            int rows = matrix.GetLength(0);   
            int cols = matrix.GetLength(1);   
            byte[] result = new byte[rows * cols];  

            int index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[index++] = matrix[i, j];  
                }
            }

            return result;
        }

        private static byte[] Xor(byte[] arr1, byte[] arr2)
        {
            byte[] result = new byte[arr1.Length];
            for (int i = 0; i < arr1.Length; i++)
            {
                result[i] = (byte)(arr1[i] ^ arr2[i]);
            }
            return result;
        }

        private static byte[] SBox(byte[] arr1)
        {
            byte[] result = new byte[arr1.Length];
            for (int i = 0; i < arr1.Length; i++)
            {
                result[i] = _sBlocks[i];
            }
            return result;
        }

        private static byte[,] ConvertToMatrix(byte[] arr)
        {
            byte[,] matrix = new byte[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrix[i, j] = arr[i * 4 + j];
                }
            }
            return matrix;
        }


        private static void ShiftRows(byte[,] s)
        {
            byte temp1 = s[1, 0];
            for (int i = 0; i < 3; i++)
            {
                s[1, i] = s[1, i + 1];
            }
            s[1, 3] = temp1;

            byte temp2 = s[2, 0];
            byte temp3 = s[2, 1];
            for (int i = 0; i < 2; i++)
            {
                s[2, i] = s[2, i + 2];
            }
            s[2, 2] = temp2;
            s[2, 3] = temp3;

            byte temp4 = s[3, 0];
            byte temp5 = s[3, 1];
            byte temp6 = s[3, 2];
            byte temp7 = s[3, 3];
            for (int i = 0; i < 3; i++)
            {
                s[3, i] = s[3, i + 1];
            }
            s[3, 3] = temp6;
            s[3, 0] = temp7;
            s[3, 1] = temp4;
            s[3, 2] = temp5;
        }


        private static void PrintMatrix(byte[,] matrix)
        {
            int rows = matrix.GetLength(0);  
            int columns = matrix.GetLength(1);  

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Console.Write(matrix[i, j].ToString("X2") + " ");  
                }
                Console.WriteLine();  
            }
        }
        static byte[,] MultiplyMatrixBy2(byte[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            byte[,] result = new byte[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = GaloisMultiplyBy2(matrix[i, j]);
                }
            }

            return result;
        }

 
        static byte GaloisMultiplyBy2(byte value)
        {
            
            byte shifted = (byte)(value << 1);

            
            if ((value & 0x80) != 0)
            {
                shifted ^= 0x1B; 
            }

            return shifted;
        }

    }
}
