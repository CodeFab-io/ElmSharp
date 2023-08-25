﻿using System.Security.Cryptography;

namespace ElmSharp;

public static partial class ElmSharp<TModel, TMessage>
{
    public abstract partial class Command 
    {
        public sealed class GetRandomNumberCommand : RunnableCommand 
        { 
            public int FromInclusive { get; init; }
            public int ToExclusive { get; init; }
            public Func<int, TMessage> OnRandomNumberGenerated { get; init; }

            public GetRandomNumberCommand(int fromInclusive, int toExclusive, Func<int, TMessage> onRandomNumberGenerated) =>
                (FromInclusive, ToExclusive, OnRandomNumberGenerated) =
                (fromInclusive, toExclusive, onRandomNumberGenerated);

            public override Task<TMessage?> Run()
            {
                var randomNumber = RandomNumberGenerator.GetInt32(
                    fromInclusive: FromInclusive,
                    toExclusive: ToExclusive);

                var message = OnRandomNumberGenerated(randomNumber);

                return Task.FromResult<TMessage?>(message);
            }
        }
    }
}
