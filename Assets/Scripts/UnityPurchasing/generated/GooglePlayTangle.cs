// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Qr/BvrlhvQ6EGxdh+Jeg1xwgk47zcH5xQfNwe3PzcHBxzSB+PVlQrv1snlOOURzjfefarvfkj5uhumYo/n6uNCtp0yJodGeY6VDnSHS/5YfkOmUxrlf4K+346SwbTWrRfP1UrErhIQGTbOP7CeK1zNuwgdtYzpH0QfNwU0F8d3hb9zn3hnxwcHB0cXIxPGsLnRFT7BXR8zGJaGzzJCEG4QtKoggHTXXrSWPDurGJTuy2ahw1bBK8S/ftL7HJozpV+a2TqAcMOLVjkEEv5/T2LetMVtReLEx+VZifdOJPUj2tJK062usdQPLoud4qQrY9VRuZzo3R7mFd0K6NaFPRRPeqCEMXABzrsRt6LKIa8LW3DTqsvqrJ65CX4ohNypiT/HNycHFw");
        private static int[] order = new int[] { 4,1,12,8,8,7,8,10,12,9,12,12,13,13,14 };
        private static int key = 113;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
