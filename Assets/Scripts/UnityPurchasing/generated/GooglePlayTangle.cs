// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("P4YTVNytB7Hb4+PnJQsJMTqXSLyz4AqeoRRIqMxjUHpOaT2MmRgDnLkLiKu5hI+Aow/BD36EiIiIjImKpbNflYjV7fEZ8pNMjRSRKti2NZGojRirhy6oLte1O7MQTkRr27uBfujN9+lvsknN35yNlDvGxDEEkePaC4iGibkLiIOLC4iIiTlHygo5HamJBPAl0zTPQxkJJkfnBjrR/iG7/faVv89Nzw9c3ks7Xv984F1YinQ6l6hXsvENbBdzjQKKhRe6bqj+t5T6N1+ywSxHp9atKHMgYYILOCWtxq82x7/BC5WLT6fSHYwjAWn0ETGH0uxR2SV5fhEEU1Q5sIxzOsdyK6Nw7pUGVS3XNqydfE2xbO8tXdTvLwIjidllaboVOouKiImI");
        private static int[] order = new int[] { 10,13,10,8,6,7,13,8,8,12,12,13,12,13,14 };
        private static int key = 137;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
