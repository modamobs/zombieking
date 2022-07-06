#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("1p0/Yq1p3CPd+r3t9F32Lw9sVZtndW1zCuscW5y1ofL33vwawGuz+B7vC8fQNqPIWP1SIPHdyR+1IqlePIxN+SoJyidY4Ia++JNP++EjAgSCg0bEAMBcoV8T/soIKLUzmSJ31dIfCvjedzpJATrlEU3Q+7bJGXXSNkuG4IUge5fwjrwQoy0ChBnBqujiqxzNykm+dQF7KNcjMnwejnADSvvamGxCeRG0/Bd9h+KJy9DZEf7tws+KzJfxqtb8EJ7HhlgZU8eS4K3fnbm358Kvj65tmw+eOrlpqohpKUbRrCHiAHQ3RUhaB1O7/ZrvDs2WLZ8cPy0QGxQ3m1Wb6hAcHBwYHR6fHBIdLZ8cFx+fHBwdiirIrM6ALBDaZjQE1SJ/Th8eHB0c");
        private static int[] order = new int[] { 2,4,7,5,8,6,13,10,9,13,11,12,12,13,14 };
        private static int key = 29;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
