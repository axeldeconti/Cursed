﻿using System;
using UnityEngine;

[Serializable]
public class TransformReference
{
    public bool UseConstant = true;
    public Transform ConstantValue;
    public TransformVariable Variable;

    public TransformReference()
    { }

    public TransformReference(Transform value)
    {
        UseConstant = true;
        ConstantValue = value;
    }

    public Transform Value
    {
        get { return UseConstant ? ConstantValue : Variable.Value; }
        set
        {
            if (UseConstant)
                ConstantValue = value;
            else
                Variable.Value = value;
        }
    }

    public static implicit operator Transform(TransformReference reference)
    {
        return reference.Value;
    }
}
