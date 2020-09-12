using System;
using System.Collections.Generic;
using System.Text;

namespace AivyDomain.UseCases
{
    internal interface IRequestHandler<in T1Request, in T2Request, in T3Request, out TResponse>
    {
        TResponse Handle(T1Request request1, T2Request request2, T3Request request3);
    }

    internal interface IRequestHandler<in T1Request, in T2Request, out TResponse>
    {
        TResponse Handle(T1Request request1, T2Request request2);
    }

    internal interface IRequestHandler<in TRequest, out TResponse>
    {
        TResponse Handle(TRequest request);
    }

    internal interface IRequestHandler<out TResponse>
    {
        TResponse Handle();
    }
}
