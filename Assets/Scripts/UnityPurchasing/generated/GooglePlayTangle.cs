// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("916EGZiuYQjMF87XYgzc4qkZ/sRn+2FwSa+/lBxnDmO+GEn5fUV4IdhVn93Rt5y+wPkrP5pBtjiLDpC0zWQBfIzmuFv4e+bOmHzX5u3zbjfJD6ccZSnnBn4quNcLOTVgBFw940lBBJ4lIpZiduVi6DG2MXmSptYC1ml9oinskmcD8EM8cAJZlr2Ro6mXFBoVJZcUHxeXFBQVi7Mj7X30zsJ1wyJNZMxda2342QKAp2bReBtqnzhMesmasnI5gMCQw4jGbU31su+GeDkM9+OqyilwjDrqbBViQIqt4fO21Pa1lhE8P5tQzxc4MHBp208lP+kCoExwkKffuF+XA9dVM6qyTJ0llxQ3JRgTHD+TXZPiGBQUFBAVFueoos5SepQAlBcWFBUU");
        private static int[] order = new int[] { 10,12,7,9,5,6,9,12,9,13,13,12,12,13,14 };
        private static int key = 21;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
