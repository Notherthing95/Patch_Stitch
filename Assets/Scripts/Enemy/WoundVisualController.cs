using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WoundVisualController : MonoBehaviour
{
    [SerializeField] DecalProjector[] woundDecals;
    BodyInfo _bodyInfo;

    float _maxLife;

    float _lostLife;

    float _previousAttackCount;

    float[] _maxHeight;

    float[] _previousHeight;

    float[] _Height;

    float _lifePerWound;

    bool _isJointPrevious;

    const float MinHeight = 0.04f;

    const float Duration = 0.17f;

    float _timer;

    bool _isChanging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _bodyInfo = GetComponent<BodyInfo>();
        _maxLife = _bodyInfo.Life;
        _maxHeight = new float[woundDecals.Length];
        _previousHeight = new float[woundDecals.Length];
        for (int i = 0; i < woundDecals.Length; i++)
        {
            _maxHeight[i] = woundDecals[i].size.y;
            _previousHeight[i] = _maxHeight[i];
        }
        _Height = new float[woundDecals.Length];
        _lifePerWound = _maxLife / woundDecals.Length;
    }

    // Update is called once per frame
    void Update()
    {
        _lostLife = _maxLife - _bodyInfo.Life;
        if (_bodyInfo.isJoint)
        {
            if (!_isJointPrevious)
            {
                _isJointPrevious = true;
            }

            if (_previousAttackCount != _bodyInfo.attackCount)
            {
                for (int i = 0; i < woundDecals.Length; i++)
                {
                    _Height[i] = Mathf.Lerp(_maxHeight[i], MinHeight, 1 - (_lifePerWound - ((_bodyInfo.attackCount + _lostLife) - _lifePerWound * (woundDecals.Length - 1 - i))) / _lifePerWound);
                }
                _isChanging = true;
            }
        }
        else if (_isJointPrevious)
        {
            for (int i = 0; i < woundDecals.Length; i++)
            {
                _Height[i] = Mathf.Lerp(_maxHeight[i], MinHeight, 1 - (_lifePerWound - (_lostLife - _lifePerWound * (woundDecals.Length - 1 - i))) / _lifePerWound);
                _isChanging = true;

                if (_Height[i] == MinHeight)
                {
                    woundDecals[i].gameObject.SetActive(false);
                }
            }
            _isJointPrevious = false;
            _previousAttackCount = 0;
        }
        _previousAttackCount = _bodyInfo.attackCount;


        if (_isChanging)
        {
            _timer += Time.deltaTime;

            for (int i = 0; i < woundDecals.Length; i++)
            {
                woundDecals[i].size = new Vector3(woundDecals[i].size.x, Mathf.Lerp(_previousHeight[i], _Height[i], _timer / Duration), woundDecals[i].size.z);
            }

            if (_timer >= Duration)
            {
                _isChanging = false;
                _timer = 0;
                for (int i = 0; i < woundDecals.Length; i++)
                {
                    _previousHeight[i] = _Height[i];
                }
            }
        }
    }
}
