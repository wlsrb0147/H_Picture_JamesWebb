using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = DebugEx;

public class NewBehaviourScript : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.speed = 0.33f;
    }
}
