using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Numerics;

namespace RingSignature.Benchmarks;

[MemoryDiagnoser]
public class RingSignerBenchmark
{
    [Params(10, 100)]
    public int RingSize;

    private KeyPairGenerator? _keyPairGenerator;
    private LsagRingSigner? _ringSigner;

    (BigInteger privateKey, BigInteger publicKey)[]? _keyPairs;
    private byte[]? _message;
    private Signature? _signature;

    [GlobalSetup(Target = nameof(SignMessage))]
    public void Setup()
    {
        PrimeOrderGroup primeOrderGroup = WellKnownPrimeOrderGroups.RFC5114_2_3_256;
        Random random = new Random();
        _keyPairGenerator = new KeyPairGenerator(primeOrderGroup, random);
        _ringSigner = new LsagRingSigner(primeOrderGroup, random);

        _keyPairs = CreateKeyPairs(RingSize);
        _message = new byte[100];
        random.Fill(_message);
    }

    [GlobalSetup(Target = nameof(VerifySignature))]
    public void SetupForVerifying()
    {
        PrimeOrderGroup primeOrderGroup = WellKnownPrimeOrderGroups.RFC5114_2_3_256;
        Random random = new Random();
        _keyPairGenerator = new KeyPairGenerator(primeOrderGroup, random);
        _ringSigner = new LsagRingSigner(primeOrderGroup, random);

        _keyPairs = CreateKeyPairs(RingSize);
        _message = new byte[100];
        random.Fill(_message);

        _signature = SignMessage();
    }

    [Benchmark]
    public Signature SignMessage()
    {
        return _ringSigner!.Sign(_message!, _keyPairs!.Select(k => k.publicKey).ToArray(), _keyPairs![0].privateKey, 0);
    }

    [Benchmark]
    public bool VerifySignature()
    {
        return _ringSigner!.Verify(_message!, _signature!, _keyPairs!.Select(k => k.publicKey).ToArray());
    }

    private (BigInteger privateKey, BigInteger publicKey)[] CreateKeyPairs(int ringSize)
    {
        (BigInteger privateKey, BigInteger publicKey)[] keyPairs = new (BigInteger privateKey, BigInteger publicKey)[ringSize];

        for (int i = 0; i < ringSize; i++)
        {
            keyPairs[i] = _keyPairGenerator!.CreateKeyPair();
        }

        return keyPairs;
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<RingSignerBenchmark>();
    }
}
