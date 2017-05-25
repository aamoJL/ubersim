using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarNavigator : MonoBehaviour {

    public GameObject minimapPlayer;        // Objekti mikä näkyy minimapissa
    public GameObject navigator;            // Objekti missä navigointinuolet

    private Transform[] navigatorArrows;
    private Transform destinationArrow;
    private Transform[] customerPositions;
    private Transform destinationPosition;

    private void Awake()
    {
        int arrows = navigator.transform.childCount;
        navigatorArrows = new Transform[arrows - 1];
        for (int i = 0; i < arrows - 1; i++)
        {
            navigatorArrows[i] = navigator.transform.GetChild(i);
        }
        destinationArrow = navigator.transform.GetChild(arrows - 1);
    }

    private void Start()
    {
        minimapPlayer.SetActive(true);
        SetCustomerArrows(true);
        SetDestinationArrow(false);
    }

    /// <summary>
    /// Sets customer navigation arrow rotation
    /// </summary>
    internal void UpdateCustomerNavigation()
    {
        SetDestinationArrow(false);
        for (int i = 0; i < navigatorArrows.Length; i++)
        {
            if (customerPositions[i] != null)
            {
                navigatorArrows[i].gameObject.SetActive(true);
                navigatorArrows[i].LookAt(customerPositions[i]);
            }
            else
            {
                navigatorArrows[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Sets destination arrow rotation
    /// </summary>
    internal void UpdateDestinationNavigation()
    {
        SetDestinationArrow(true);
        SetCustomerArrows(false);
        destinationArrow.LookAt(customerPositions[3]);
    }

    /// <summary>
    /// Spawnpoint positions to navigator
    /// </summary>
    internal void SetDestinations(Transform[] customerObjects)
    {
        customerPositions = customerObjects;
    }

    /// <summary>
    /// Sets customer navigation arrow active or inactive
    /// </summary>
    private void SetCustomerArrows(bool v)
    {
        for (int i = 0; i < navigatorArrows.Length; i++)
        {
            navigatorArrows[i].gameObject.SetActive(v);
        }
    }

    /// <summary>
    /// Sets destination arrow active or inactive
    /// </summary>
    private void SetDestinationArrow(bool v)
    {
        destinationArrow.gameObject.SetActive(v);
    }
}
