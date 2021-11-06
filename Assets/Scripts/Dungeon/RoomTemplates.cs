using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    [Header("Templates")]
    public GameObject m_BlockedRoom;
    public GameObject m_MiddleRoom;
    public GameObject[] m_TopRooms;
    public GameObject[] m_LeftRooms;
    public GameObject[] m_BottomRooms;
    public GameObject[] m_RightRooms;

    [Header("Covers")]
    public GameObject m_Cover;
    public GameObject m_TopCover;
    public GameObject m_LeftCover;
    public GameObject m_BottomCover;
    public GameObject m_RightCover;

    [Header("Layout")]
    public GameObject m_Stairs;
    public GameObject m_HorizontalMarker; // Used to mark rooms the player has visited
    public GameObject m_VerticalMarker; // Used to mark rooms the player has visited

    [Header("Decor")]
    public GameObject m_Rug;
    public List<GameObject> m_RoomsDecor = new List<GameObject>();

    [Header("Treasures")]
    public GameObject m_Treasure;
    public GameObject m_HealthPickup;
}
