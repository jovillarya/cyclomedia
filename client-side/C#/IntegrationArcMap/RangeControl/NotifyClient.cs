namespace RangeSelector
{
  /// <RangeSelectorControl_Resize>
  /// The below is the small Notification class that can be used by the client
  /// </RangeSelectorControl_Resize>
  /// 
  #region Notification class for client to register with the control for changes

  public class NotifyClient
  {
    public string Range1 { get; set; }

    public string Range2 { get; set; }
  }

  #endregion
}
