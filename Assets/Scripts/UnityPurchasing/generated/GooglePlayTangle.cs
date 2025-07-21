// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("93R6dUX3dH9393R0dckkejldVKpRH53KidXqZVnUqolsV9VA864MR0a7xbq9ZbkKgB8TZfyTpNMYJJeKTuUlBZdo5/8N5rHI37SF31zKlfATBBjvtR9+KKYe9LGzCT6ouq7N72eURSvj8PIp70hS0FooSHpRnJtw+WiaV4pVGOd5496q8+CLn6W+YixF93RXRXhzfF/zPfOCeHR0dHB1dvp6qjAvbdcmbHBjnO1U40xwu+GD4D5hNapT/C/p/O0oH0lu1Xj5UKg1OG8PmRVX6BHV9zWNbGj3ICUC5WgWuE/z6Su1zac+Uf2pl6wDCDyx5ktWOakgqT7e7xlE9uy92i5GsjkPTqYMA0lx701nx761jUrosm4YMZST5oxJzpyX+Hd2dHV0");
        private static int[] order = new int[] { 1,13,4,7,11,6,12,13,8,13,10,13,12,13,14 };
        private static int key = 117;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
