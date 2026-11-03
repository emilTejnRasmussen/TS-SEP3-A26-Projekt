using Entities;


namespace Tests_UserServer;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        User user1 = new User("TomBoy", "migthai@headache.weeee");
        User user2 = new User("Jailbait", "oneSome@bankok.nudl");
        User user3 = new User("Candy", "FairAndSquare@NotTonight.we");
        User user4 = new User("MamboNumber5", "Tango@yeas.dj");
        
        
        
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}