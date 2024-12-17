using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class InvalidePropertyException : Exception
{
    public InvalidePropertyException() : base(){}
    public InvalidePropertyException(string message) : base(message){}
    public InvalidePropertyException(string message, Exception innerException) : base(message, innerException){}
    protected InvalidePropertyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context){}

}