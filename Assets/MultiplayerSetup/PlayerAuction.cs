using System;

[Serializable]
public class PlayerAuction
{
    public string _id;
    public string seller;
    public string sellerResourceType;
    public int sellerAmount;
    public string buyerResourceType;
    public int buyerAmount;
    public bool state;
}
