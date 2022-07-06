#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("mYWMybuGhp3JqqjZ9/7k2d/Z3duenseImZmFjMeKhoTGiJmZhYyKiO4FlNBqYrrJOtEtWFZzpuOCFsIVfHeT5U2uYrI9/97aIi3mpCf9gDhCSph7rrq8KEbGqFoREgqZJA9KpeZ01BrCoMHzIRcnXFDnMLf1PyLUXNNEHebn6XviWMj/x5081eQyi//Jho/JnYGMyZ2BjIfJiJmZhYCKiPZsamzycNSu3htAcqlnxT1Yefsx9ngy9665AuwEt5BtxALfS76lvAXJqqjZa+jL2eTv4MNvoW8e5Ojo6M/Zze/qvO3i+vSomZmFjMmqjJudoDGfdtr9jEiefSDE6+ro6ehKa+jt7/rrvLrY+tn47+q87eP646iZmY3cyvyi/LD0Wn0eH3V3JrlTKLG5lqhBcRA4I491zYL4OUpSDfLDKvadgYabgJ2Q2P/Z/e/qvO3q+uSomYCPgIqInYCGh8monJ2BhpuAnZDY2WvtUtlr6kpJ6uvo6+vo69nk7+Ds6epr6Obp2Wvo4+tr6OjpDXhA4IuFjMmanYiHjYibjcmdjJuEmsmIIPCbHLTnPJa2chvM6lO8ZqS05Bhe8lR6q837wy7m9F+kdbeKIaJp/jDflihuvDBOcFDbqxIxPJh3l0i74cLv6Ozs7uvo//eBnZ2ZmtPGxp7G2Wgq7+HC7+js7O7r69loX/NoWp2Aj4CKiJ2MyYuQyYiHkMmZiJudQTWXy9wjzDww5j+CPUvNyvgeSEXDb6FvHuTo6Ozs6dmL2OLZ4O/qvNTPjslj2oMe5GsmNwJKxhC6g7KN3NvY3dna37P+5Nrc2dvZ0NvY3dnFyYqMm52Aj4CKiJ2MyZmGhYCKkFjZsQWz7dtlgVpm9DeMmhaOt4xVrJf2pYK5f6hgLZ2L4vlqqG7aY2jHqU8erqSW4bfZ9u/qvPTK7fHZ/2vo6e/gw2+hbx6Kjezo2Wgb2cPvkMmImpqchIyayYiKioyZnYiHiozv2ebv6rz0+ujoFu3s2ero6BbZ9CmK2p4e0+7FvwIz5sjnM1Oa8KZck9lr6J/Z5+/qvPTm6OgW7e3q6+jNCwI4Xpk25qwIziMYhJEEDlz+/lcdmnIHO43mIpCm3TFL1xCRFoIhhYzJoIeKx9jP2c3v6rzt4vr0qJn/2f3v6rzt6vrkqJmZhYzJu4aGnZmFjMmqjJudgI+AioidgIaHyaic7+q89Oft/+39wjmArn2f4BcdgmTZ+O/qvO3j+uOomZmFjMmgh4rH2OTv4MNvoW8e5Ojo7Ozp6mvo6Om1af3COYCufZ/gFx2CZMepTx6upJbJiIeNyYqMm52Aj4CKiJ2AhofJmeG32Wvo+O/qvPTJ7Wvo4dlr6O3Z2t+z2YvY4tng7+q87e/667y62PqHjcmKhoeNgJ2AhoeayYaPyZyajLBO7OCV/qm/+PedOl5iytKuSjyGm4iKnYCKjMmanYidjISMh52ax9m7jIWAiIeKjMmGh8mdgYCayYqMm45m4V3JHiJFxcmGmV/W6NllXqomYvBgNxCihRzuQsvZ6wHx1xG54DpmmmiJL/Ky4MZ7WxGtoRmJ0Xf8HN9wpcSRXgRlcjUannIbnzue2aYouUNjPDMNFTng7t5ZnJzI");
        private static int[] order = new int[] { 6,36,51,18,24,22,23,41,49,15,13,13,27,51,20,17,22,51,23,20,42,42,59,58,35,37,39,54,36,56,41,44,39,37,44,44,40,56,57,40,53,50,58,54,44,50,51,47,53,50,56,56,54,54,56,57,58,59,59,59,60 };
        private static int key = 233;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
