using System;

namespace ComputingPi
{
    public interface IComputePi
    {
        string Name { get; }
        double ComputePi(int numberOfSteps);
    }
}
