namespace MD5.Tests;

public class Tests
{
    CheckSum checkSum = new ();

    [Test]
    public async Task ParallelAndSubsequentHashCalculationsGiveSameResult()
    {
        var path = "../../../TestFiles/";
        var sumSequential = checkSum.CalculateSequential(path);
        Thread.Sleep(1000);

        var sumParallel = await checkSum.CalculateParallel(path);
        Assert.That(sumSequential, Is.EqualTo(sumParallel));
    }
}