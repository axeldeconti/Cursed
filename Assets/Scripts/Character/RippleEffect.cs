using UnityEngine;
using System.Collections;

namespace Cursed.FX
{
    [RequireComponent(typeof(Camera))]
    public class RippleEffect : MonoBehaviour
    {
        public AnimationCurve waveform = new AnimationCurve(
            new Keyframe(0.00f, 0.50f, 0, 0),
            new Keyframe(0.05f, 1.00f, 0, 0),
            new Keyframe(0.15f, 0.10f, 0, 0),
            new Keyframe(0.25f, 0.80f, 0, 0),
            new Keyframe(0.35f, 0.30f, 0, 0),
            new Keyframe(0.45f, 0.60f, 0, 0),
            new Keyframe(0.55f, 0.40f, 0, 0),
            new Keyframe(0.65f, 0.55f, 0, 0),
            new Keyframe(0.75f, 0.46f, 0, 0),
            new Keyframe(0.85f, 0.52f, 0, 0),
            new Keyframe(0.99f, 0.50f, 0, 0)
        );

        [Range(0.01f, 1.0f)]
        public FloatReference refractionStrength;//0.5f

        public Color reflectionColor = Color.gray;

        [Range(0.01f, 1.0f)]
        public FloatReference reflectionStrength;//0.7f

        [Range(1.0f, 5.0f)]
        public FloatReference waveSpeed;//1.25f

        [Range(0.0f, 2.0f)]
        public FloatReference dropInterval;//0.5f

        [SerializeField, HideInInspector]
        private Shader _shader;

        private Camera _cam;
        private Droplet[] _droplets;
        private Texture2D _gradTexture;
        private Material _material;
        private float _timer;
        private int _dropCount;

        void UpdateShaderParameters()
        {
            _material.SetVector("_Drop1", _droplets[0].MakeShaderParameter(_cam.aspect));
            _material.SetVector("_Drop2", _droplets[1].MakeShaderParameter(_cam.aspect));
            _material.SetVector("_Drop3", _droplets[2].MakeShaderParameter(_cam.aspect));

            _material.SetColor("_Reflection", reflectionColor);
            _material.SetVector("_Params1", new Vector4(_cam.aspect, 1, 1 / waveSpeed, 0));
            _material.SetVector("_Params2", new Vector4(1, 1 / _cam.aspect, refractionStrength, reflectionStrength));
        }

        void Awake()
        {
            _cam = GetComponent<Camera>();

            _droplets = new Droplet[3];
            _droplets[0] = new Droplet();
            _droplets[1] = new Droplet();
            _droplets[2] = new Droplet();

            _gradTexture = new Texture2D(2048, 1, TextureFormat.Alpha8, false);
            _gradTexture.wrapMode = TextureWrapMode.Clamp;
            _gradTexture.filterMode = FilterMode.Bilinear;
            for (var i = 0; i < _gradTexture.width; i++)
            {
                var x = 1.0f / _gradTexture.width * i;
                var a = waveform.Evaluate(x);
                _gradTexture.SetPixel(i, 0, new Color(a, a, a, a));
            }
            _gradTexture.Apply();

            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
            _material.SetTexture("_GradTex", _gradTexture);

            UpdateShaderParameters();
        }

        void Update()
        {
            if (dropInterval > 0)
            {
                _timer += Time.deltaTime;
                while (_timer > dropInterval)
                {
                    //Emit();
                    _timer -= dropInterval;
                }
            }

            foreach (var d in _droplets) d.Update();

            UpdateShaderParameters();
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, _material);
        }

        public void Emit(Vector2 pos)
        {
            _droplets[_dropCount++ % _droplets.Length].Reset(pos);
        }

        IEnumerator Stop()
        {
            yield return new WaitForSeconds(.3f);
        }

    }
}