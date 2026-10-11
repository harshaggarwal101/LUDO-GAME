using System.Collections.Generic;
using UnityEngine;

public class PathObjectParent : MonoBehaviour
{
    public PathPoint[] OrdinaryPathPoints;
    public PathPoint[] BluePathPoints;
    public PathPoint[] RedPathPoints;
    public PathPoint[] GreenPathPoints;
    public PathPoint[] YellowPathPoints;
    public Transform[] BasePoints;
    public List<PathPoint> safePathPoints;
    public float[] scales;
    public float[] posDiff;
}
