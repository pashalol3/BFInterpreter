namespace BFCompiler
{
    enum TokenKind
    {
        Inc,
        Dec,
        MoveLeft,
        MoveRight,
        Print,
        Read,
        LoopStart,
        LoopEnd
    }

    internal struct GroupedSymbols(TokenKind token, int count, int addres = -1, bool isLoop = false)
    {
        public int Count { get; } = count;
        public TokenKind Token { get; } = token;
        public int InstructionAddress { get; set; } = addres;

        public override string ToString()
        {
            return $"'{Token}' ({Count:D2}) [{InstructionAddress}]";
        }
    }
}
