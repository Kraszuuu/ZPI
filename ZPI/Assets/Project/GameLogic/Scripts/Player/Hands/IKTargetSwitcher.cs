using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;  // Konieczne, aby korzystać z Two Bone IK Constraint

public class IKTargetSwitcher : MonoBehaviour
{
    public Transform defaultTarget;    // Punkt docelowy dla domyślnej pozycji ręki
    public Transform holdingTarget;    // Punkt docelowy dla pozycji trzymania przedmiotu
    public TwoBoneIKConstraint ikConstraint;  // Referencja do Two Bone IK Constraint

    private bool isHolding = false;    // Flaga przechowująca stan (czy trzyma przedmiot)

    void Start()
    {
        // Ustaw domyślny punkt jako startowy
        ikConstraint.data.target = defaultTarget;
    }

    void Update()
    {
        // Sprawdź, czy wciśnięto przycisk (np. klawisz "H" do przełączania)
        if (Input.GetKeyDown(KeyCode.H))
        {
            // Przełącz pomiędzy domyślnym a punktem trzymania przedmiotu
            isHolding = !isHolding;

            // Zmień target w zależności od stanu
            ikConstraint.data.target = isHolding ? holdingTarget : defaultTarget;
        }
    }
}

