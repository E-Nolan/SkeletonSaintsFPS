using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObject : MonoBehaviour
{
    public List<Renderer> Renderers = new List<Renderer>();
    public List<Material> Materials = new List<Material>();
    public Vector3 Position;

    [HideInInspector]
    public float InitialAlpha;

    private void Awake()
    {
        Position = transform.position;

        if (Renderers.Count == 0)
        {
            Renderers.AddRange(GetComponentsInChildren<Renderer>());
        }

        foreach (Renderer rend in Renderers)
        {
            Materials.AddRange(rend.materials);
        }

        InitialAlpha = Materials[0].color.a;
    }

    public bool Equals(FadeObject other)
    {
        return Position.Equals(other.Position);
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
}
