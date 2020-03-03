using UnityEngine;

namespace Cursed.Traps
{
    public class ElectricArc : MonoBehaviour
    {
        public int segments;
        public float xradius;
        public float yradius;
        LineRenderer line;

        private void Start()
        {
            line = GetComponent<LineRenderer>();
            line.SetVertexCount(segments + 1);
            line.useWorldSpace = false;
        }

        //function for firing the laser
        public void Fire()
        {
            float x = 0f;
            float y = 0f;
            float z;
            float angle = 20f;

            for (int i = 0; i < (segments + 1); i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

                line.SetPosition(i, new Vector3(x, y, z));

                angle += (180f / segments);
            }
        }
    }
}


