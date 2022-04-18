interface IDamageReceiver
{
    float Health { get; set; }

    public void Receive(float damage);
}
