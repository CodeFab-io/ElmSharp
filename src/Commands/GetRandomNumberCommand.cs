using System.Security.Cryptography;

namespace ElmSharp;

public static partial class ElmSharp<TModel, TMessage>
{
    public abstract partial class Command 
    {
        /// <summary>
        /// This command will generate a new random number between <see cref="FromInclusive"/> and <see cref="ToExclusive">.
        /// Internally uses <see cref="RandomNumberGenerator.GetInt32(int, int)"/>.
        /// If ToExclusive is lower or equal to FromInclusive, FromInclusive will be returned.
        /// </summary>
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
                var randomNumber = 
                    ToExclusive <= FromInclusive 
                    ? FromInclusive 
                    : RandomNumberGenerator.GetInt32(
                        fromInclusive: FromInclusive,
                        toExclusive: ToExclusive);

                var message = OnRandomNumberGenerated(randomNumber);

                return Task.FromResult<TMessage?>(message);
            }
        }
    }
}
