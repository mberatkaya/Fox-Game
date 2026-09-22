using UnityEngine;

namespace TilkiOyunu.Foundation.Editor
{
    public static class ForestBridgePlacement
    {
        public static void AlignToBanks(Transform bridge, Terrain terrain, Vector3 center, float yaw, float length, float width)
        {
            Quaternion heading = Quaternion.Euler(0f, yaw, 0f);
            Vector3 forward = heading * Vector3.forward;
            Vector3 right = heading * Vector3.right;
            Vector3 bankA = center - forward * (length * 0.5f);
            Vector3 bankB = center + forward * (length * 0.5f);
            bankA.y = BankHeight(terrain, bankA, right, width) + 0.06f;
            bankB.y = BankHeight(terrain, bankB, right, width) + 0.06f;
            Quaternion rotation = Quaternion.LookRotation(bankB - bankA, Vector3.up);
            // The walkable slab's top is 0.28 above the bridge origin.
            bridge.SetPositionAndRotation((bankA + bankB) * 0.5f - rotation * (Vector3.up * 0.28f), rotation);
            bridge.localScale = Vector3.one;
        }

        private static float BankHeight(Terrain terrain, Vector3 center, Vector3 right, float width)
        {
            float height = float.NegativeInfinity;
            for (int i = -1; i <= 1; i++)
            {
                Vector3 point = center + right * (i * width * 0.5f);
                height = Mathf.Max(height, terrain.transform.position.y + terrain.SampleHeight(point));
            }
            return height;
        }
    }
}
