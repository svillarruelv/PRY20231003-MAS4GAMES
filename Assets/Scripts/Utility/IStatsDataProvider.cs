using UnityEngine;
using UnityEngine.UI;

public interface IStatsDataProvider
{
  StatsData GetStatsData();
  Vector3 GetPosition();
  float GetMainMetric();

}