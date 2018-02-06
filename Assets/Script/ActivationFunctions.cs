using System;

///<summary>
///Activation Functions from:
///https://en.wikipedia.org/wiki/Activation_function
///https://stats.stackexchange.com/questions/115258/comprehensive-list-of-activation-functions-in-neural-networks-with-pros-cons
///D infront means the Deravitive of the function
///x is the input of one perceptron. a is the alpha value sometimes needed.
///</summary>
[System.Serializable]
static public class ActivationFunctions
{
    static public double AFunction(double x, ActivationType activationType)
    {
        switch (activationType)
        {
            case ActivationType.Identity:
                return Identity(x);
            case ActivationType.BinaryStep:
                return BinaryStep(x);
            case ActivationType.Logistic:
                return Logistic(x);
            case ActivationType.Tanh:
                return Tanh(x);
            case ActivationType.ArcTan:
                return ArcTan(x);
            case ActivationType.ReLU:
                return ReLU(x);
            case ActivationType.SoftPlus:
                return SoftPlus(x);
            case ActivationType.BentIdentity:
                return BentIdentity(x);
            case ActivationType.Sinusoid:
                return Sinusoid(x);
            case ActivationType.Sinc:
                return Sinc(x);
            case ActivationType.Gaussian:
                return Gaussian(x);
            case ActivationType.Bipolar:
                return Bipolar(x);
            case ActivationType.BipolarSigmoid:
                return BipolarSigmoid(x);
            case ActivationType.Sigmoid:
                return Sigmoid(x);
        }
        return 0;
    }

    static private double Identity(double x)
    {
        return x;
    }
    static private double BinaryStep(double x)
    {
        return x < 0 ? 0 : 1;
    }
    static private double Logistic(double x)
    {
        return 1 / (1 + Math.Pow(Math.E, -x));
    }
    static private double Tanh(double x)
    { 
        return Math.Tanh(x);
    }
    static private double ArcTan(double x)
    {
        return Math.Atan(x);
    }
    static private double ReLU(double x)
    {
        return Math.Max(0, x);// x < 0 ? 0 : x;
    }
    static private double SoftPlus(double x)
    {
        return Math.Log(Math.Exp(x) + 1);
    }
    static private double BentIdentity(double x)
    {
        return (((Math.Sqrt(Math.Pow(x, 2) + 1)) - 1) / 2) + x;
    }
    static private double Sinusoid(double x)
    {
        return Math.Sin(x);
    }
    static private double Sinc(double x)
    {
        return x == 0 ? 1 : Math.Sin(x) / x;
    }
    static public double Sigmoid(double x)
    {
        return 2 / ((1 + Math.Exp(-2 * x) - 1));
    }
    static private double Gaussian(double x)
    {
        return Math.Pow(Math.E, Math.Pow(-x, 2));
    }
    static private double Bipolar(double x)
    {
        return x < 0 ? -1 : 1;
    }
    static private double BipolarSigmoid(double x)
    {
        return (1 - Math.Exp(-x)) / (1 + Math.Exp(-x));
    }



    //static private double PReLU(double x, double a)
    //{
    //    return x < 0 ? a * x : x;
    //}
    //static private double ELU(double x, double a)
    //{
    //    return x < 0 ? a * (Math.Pow(Math.E, x) - 1) : x;
    //}
    //static private double Scaler(double x, double min, double max)
    //{
    //    return (x - min) / (max - min);
    //}
}
