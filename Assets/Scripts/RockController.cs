// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;

public enum Size {
    Large, Medium, Small, Tiny
}

[RequireComponent(typeof(RandomDirection))]
[RequireComponent(typeof(WraparoundMovement))]    
public class RockController : MonoBehaviour
{
    [SerializeField] private Size size = Size.Large;
    
    public Size Size => size;
}
