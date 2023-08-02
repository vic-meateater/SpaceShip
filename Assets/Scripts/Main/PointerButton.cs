using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointerButton : Button
{    
    private Sequence _showSequence;
    private Sequence _hideSequence;
    private string _nameID;
    private float _mainScale = 1f;

    protected override void Start()
    {
        base.Start();
        _nameID = gameObject.name;
    }

    public new void OnEnable()
    {
        gameObject.transform.localScale = new Vector3(1, 1, 1);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        IncreaseScale();
    }

    public void IncreaseScale()
    {
        DOTween.Kill($"ScaleDown {_nameID}");
        _showSequence = DOTween.Sequence();
        _showSequence.SetId($"ScaleUp {_nameID}");
        _showSequence.Append(gameObject.transform.DOScale(1.1f, 0.2f));
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        DecreaseScale();
    }

    public void DecreaseScale()
    {
        DOTween.Kill($"ScaleUp {_nameID}");
        _hideSequence = DOTween.Sequence();
        _hideSequence.SetId($"ScaleDown {_nameID}");
        _hideSequence.Append(gameObject.transform.DOScale(_mainScale, 0.2f));
    }

    public void SetMainScale(float scale)
    {
        _mainScale = scale;
    }
}
