using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shazam
{
    public class IceKey
    {
        /* ATTENTION!
         * This class was decompiles by ILSpy, a .NET decompiler which is
         * quite bad in terms of names of stuff, so the original varibales
         * name were diffrent. Anyway, it was used due to "unsafe" errors.
         */

        private int size;
        private int rounds;
        private int[,] keySchedule;
        private static ulong[,] spBox;
        private static bool spBoxInitialised = false;
        private static int[,] sMod = new int[,]
		{
			
			{
				333,
				313,
				505,
				369
			},
			
			{
				379,
				375,
				319,
				391
			},
			
			{
				361,
				445,
				451,
				397
			},
			
			{
				397,
				425,
				395,
				505
			}
		};

        private static int[,] sXor = new int[,]
		{
			
			{
				131,
				133,
				155,
				205
			},
			
			{
				204,
				167,
				173,
				65
			},
			
			{
				75,
				46,
				212,
				51
			},
			
			{
				234,
				203,
				46,
				4
			}
		};

        private static uint[] pBox = new uint[]
		{
			1u,
			128u,
			1024u,
			8192u,
			524288u,
			2097152u,
			16777216u,
			1073741824u,
			8u,
			32u,
			256u,
			16384u,
			65536u,
			8388608u,
			67108864u,
			536870912u,
			4u,
			16u,
			512u,
			32768u,
			131072u,
			4194304u,
			134217728u,
			268435456u,
			2u,
			64u,
			2048u,
			4096u,
			262144u,
			1048576u,
			33554432u,
			2147483648u
		};

        private static int[] keyrot = new int[]
		{
			0,
			1,
			2,
			3,
			2,
			1,
			3,
			0,
			1,
			3,
			2,
			0,
			3,
			1,
			0,
			2
		};

        private int gf_mult(int a, int b, int m)
        {
            int num = 0;
            while (b != 0)
            {
                if ((b & 1) != 0)
                {
                    num ^= a;
                }
                a <<= 1;
                b >>= 1;
                if (a >= 256)
                {
                    a ^= m;
                }
            }
            return num;
        }

        private long gf_exp7(int b, int m)
        {
            if (b == 0)
            {
                return 0L;
            }
            int num = this.gf_mult(b, b, m);
            num = this.gf_mult(b, num, m);
            num = this.gf_mult(num, num, m);
            return (long)this.gf_mult(b, num, m);
        }

        private long perm32(long x)
        {
            long num = 0L;
            long num2 = 0L;
            while (x != 0L)
            {
                if ((x & 1L) != 0L)
                {
                    num |= (long)((ulong)IceKey.pBox[(int)checked((IntPtr)num2)]);
                }
                num2 += 1L;
                x >>= 1;
            }
            return num;
        }

        private void spBoxInit()
        {
            IceKey.spBox = new ulong[4, 1024];
            for (int i = 0; i < 1024; i++)
            {
                int num = i >> 1 & 255;
                int num2 = (i & 1) | (i & 512) >> 8;
                long x = this.gf_exp7(num ^ IceKey.sXor[0, num2], IceKey.sMod[0, num2]) << 24;
                IceKey.spBox[0, i] = (ulong)this.perm32(x);
                x = this.gf_exp7(num ^ IceKey.sXor[1, num2], IceKey.sMod[1, num2]) << 16;
                IceKey.spBox[1, i] = (ulong)this.perm32(x);
                x = this.gf_exp7(num ^ IceKey.sXor[2, num2], IceKey.sMod[2, num2]) << 8;
                IceKey.spBox[2, i] = (ulong)this.perm32(x);
                x = this.gf_exp7(num ^ IceKey.sXor[3, num2], IceKey.sMod[3, num2]);
                IceKey.spBox[3, i] = (ulong)this.perm32(x);
            }
        }

        public IceKey(int level)
        {
            if (!IceKey.spBoxInitialised)
            {
                this.spBoxInit();
                IceKey.spBoxInitialised = true;
            }
            if (level < 1)
            {
                this.size = 1;
                this.rounds = 8;
            }
            else
            {
                this.size = level;
                this.rounds = level * 16;
            }
            this.keySchedule = new int[this.rounds, 3];
        }

        private void scheduleBuild(int[] kb, int n, int krot_idx)
        {
            for (int i = 0; i < 8; i++)
            {
                int num = IceKey.keyrot[krot_idx + i];
                for (int j = 0; j < 3; j++)
                {
                    this.keySchedule[n + i, j] = 0;
                }
                for (int j = 0; j < 15; j++)
                {
                    int num2 = j % 3;
                    for (int k = 0; k < 4; k++)
                    {
                        int num3 = kb[num + k & 3];
                        int num4 = num3 & 1;
                        this.keySchedule[n + i, num2] = (this.keySchedule[n + i, num2] << 1 | num4);
                        kb[num + k & 3] = (num3 >> 1 | (num4 ^ 1) << 15);
                    }
                }
            }
        }

        public void set(char[] key)
        {
            int[] array = new int[4];
            if (this.rounds == 8)
            {
                for (int i = 0; i < 4; i++)
                {
                    array[3 - i] = (int)((int)(key[i * 2] & 'ÿ') << 8 | (key[i * 2 + 1] & 'ÿ'));
                }
                this.scheduleBuild(array, 0, 0);
                return;
            }
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    array[3 - j] = (int)((int)key[i * 8 + j * 2] << 8 | key[i * 8 + j * 2 + 1]);
                }
                this.scheduleBuild(array, i * 8, 0);
                this.scheduleBuild(array, this.rounds - 8 - i * 8, 8);
            }
        }

        public void clear()
        {
            for (int i = 0; i < this.rounds; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    this.keySchedule[i, j] = 0;
                }
            }
        }

        private ulong roundFunc(ulong p, int i, int[,] subkey)
        {
            ulong num = (p >> 16 & 1023uL) | ((p >> 14 | p << 18) & 1047552uL);
            ulong num2 = (p & 1023uL) | (p << 2 & 1047552uL);
            ulong num3 = (ulong)((long)subkey[i, 2] & (long)(num ^ num2));
            ulong num4 = num3 ^ num2;
            num3 ^= num;
            num3 ^= (ulong)((long)subkey[i, 0]);
            num4 ^= (ulong)((long)subkey[i, 1]);
            return checked(IceKey.spBox[(int)((IntPtr)0L), (int)((IntPtr)(num3 >> 10))] | IceKey.spBox[(int)((IntPtr)1L), (int)((IntPtr)(num3 & 1023uL))] | IceKey.spBox[(int)((IntPtr)2L), (int)((IntPtr)(num4 >> 10))] | IceKey.spBox[(int)((IntPtr)3L), (int)((IntPtr)(num4 & 1023uL))]);
        }

        private void encrypt(byte[] plaintext, byte[] ciphertext, int idx)
        {
            ulong num = (ulong)plaintext[idx] << 24 | (ulong)plaintext[idx + 1] << 16 | (ulong)plaintext[idx + 2] << 8 | (ulong)plaintext[idx + 3];
            ulong num2 = (ulong)plaintext[idx + 4] << 24 | (ulong)plaintext[idx + 5] << 16 | (ulong)plaintext[idx + 6] << 8 | (ulong)plaintext[idx + 7];
            for (int i = 0; i < this.rounds; i += 2)
            {
                num ^= this.roundFunc(num2, i, this.keySchedule);
                num2 ^= this.roundFunc(num, i + 1, this.keySchedule);
            }
            for (int i = 0; i < 4; i++)
            {
                ciphertext[idx + 3 - i] = (byte)(num2 & 255uL);
                ciphertext[idx + 7 - i] = (byte)(num & 255uL);
                num2 >>= 8;
                num >>= 8;
            }
        }

        private void decrypt(byte[] ciphertext, byte[] plaintext)
        {
            ulong num = (ulong)ciphertext[0] << 24 | (ulong)ciphertext[1] << 16 | (ulong)ciphertext[2] << 8 | (ulong)ciphertext[3];
            ulong num2 = (ulong)ciphertext[4] << 24 | (ulong)ciphertext[5] << 16 | (ulong)ciphertext[6] << 8 | (ulong)ciphertext[7];
            for (int i = this.rounds - 1; i > 0; i -= 2)
            {
                num ^= this.roundFunc(num2, i, this.keySchedule);
                num2 ^= this.roundFunc(num, i - 1, this.keySchedule);
            }
            for (int i = 0; i < 4; i++)
            {
                plaintext[3 - i] = (byte)(num2 & 255uL);
                plaintext[7 - i] = (byte)(num & 255uL);
                num2 >>= 8;
                num >>= 8;
            }
        }

        public int keySize()
        {
            return this.size * 8;
        }

        public int blockSize()
        {
            return 8;
        }

        public char[] encString(string str)
        {
            char[] array = str.ToCharArray();
            int length = str.Length;
            int num = (length / 8 + 1) * 8;
            byte[] array2 = new byte[num];
            byte[] array3 = new byte[num];
            for (int i = 0; i < num; i++)
            {
                array2[i] = 0;
            }
            for (int j = 0; j < length; j++)
            {
                array2[j] = (byte)array[j];
            }
            for (int k = 0; k < num; k += 8)
            {
                this.encrypt(array2, array3, k);
            }
            string text = "#0x";
            for (int l = 0; l < num; l++)
            {
                int num2 = (int)array3[l];
                text += string.Format("{0:x2}", new object[]
				{
					Convert.ToUInt32(num2.ToString())
				});
            }
            return text.ToCharArray();
        }

        public byte[] encBinary(byte[] data, int data_size)
        {
            int num = (data_size / 8 + 1) * 8;
            byte[] array = new byte[num];
            byte[] array2 = new byte[num];
            for (int i = 0; i < data_size; i++)
            {
                array[i] = data[i];
            }
            for (int j = 0; j < num; j += 8)
            {
                this.encrypt(array, array2, j);
            }
            return array2;
        }

        public string decString(string str)
        {
            str = str.Substring("#0x".Length);
            StringBuilder stringBuilder = new StringBuilder();
            char[] array = str.ToCharArray();
            int num = array.Length;
            for (int i = 0; i < num; i += 16)
            {
                byte[] array2 = new byte[8];
                byte[] array3 = new byte[8];
                for (int j = 0; j < 8; j++)
                {
                    array2[j] = Convert.ToByte(String.Concat(array[i + j * 2], array[i + j * 2 + 1]), 16);
                }
                this.decrypt(array2, array3);
                for (int k = 0; k < 8; k++)
                {
                    if (array3[k] != 0)
                    {
                        stringBuilder.Append(Convert.ToChar(array3[k]));
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
}
