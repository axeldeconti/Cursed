using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cursed.FX
{
    public class Droplet
    {
        private Vector2 _position;
        private float _time;

        public Droplet()
        {
            _time = 1000;
        }

        public void Reset(Vector2 pos)
        {
            _position = pos;
            _time = 0;
        }

        public void Update()
        {
            _time += Time.deltaTime * 2;
        }

        public Vector4 MakeShaderParameter(float aspect)
        {
            return new Vector4(_position.x * aspect, _position.y, _time, 0);
        }
    }
}