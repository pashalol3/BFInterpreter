namespace BFCompiler
{
    internal class BFInterpreter
    {
        private static readonly char[] VALID_TOKENS = ['+', '-', '>', '<', '.', ',', '[', ']'];
        private readonly Dictionary<char, TokenKind> _tokenMap =
            new()
            {
                { '+', TokenKind.Inc },
                { '-', TokenKind.Dec },
                { '<', TokenKind.MoveLeft },
                { '>', TokenKind.MoveRight },
                { '.', TokenKind.Print },
                { ',', TokenKind.Read },
                { '[', TokenKind.LoopStart },
                { ']', TokenKind.LoopEnd }
            };

        private readonly string _sourceCode;
        private IList<GroupedSymbols> _parsedInstructions;
        private int _cursor = 0;

        public BFInterpreter(string content)
        {
            _sourceCode = content;
            ParseSourceCode().BackPatch().Execute();
        }

        private BFInterpreter BackPatch()
        {
            var loopStack = new int[_parsedInstructions.Count];
            int stackPointer = 0;

            for (
                int instructionPointer = 0;
                instructionPointer < _parsedInstructions.Count;
                instructionPointer++
            )
            {
                if (_parsedInstructions[instructionPointer].Token == TokenKind.LoopStart)
                {
                    loopStack[stackPointer] = instructionPointer;
                    stackPointer++;
                }
                else if (_parsedInstructions[instructionPointer].Token == TokenKind.LoopEnd)
                {
                    if (stackPointer == 0)
                        throw new Exception("Unmatched ]");
                    stackPointer--;
                    int loopStart = loopStack[stackPointer];

                    _parsedInstructions[loopStart] = _parsedInstructions[loopStart] with
                    {
                        InstructionAddress = instructionPointer + 1
                    };
                    _parsedInstructions[instructionPointer] = _parsedInstructions[
                        instructionPointer
                    ] with
                    {
                        InstructionAddress = loopStart + 1
                    };
                }
            }

            if (stackPointer > 0)
                throw new Exception("Unmatched [");
            return this;
        }

        private BFInterpreter ParseSourceCode()
        {
            var grouped = new List<GroupedSymbols>();
            var currentAddres = 0;
            while (NextToken(out char startToken))
            {
                var repsCount = 1;
                var tempCursor = _cursor;
                while (
                    NextToken(out char currentToken)
                    && !IsLoop(currentToken)
                    && startToken == currentToken
                )
                {
                    tempCursor = _cursor;
                    repsCount++;
                }
                _cursor = tempCursor;
                grouped.Add(
                    new(_tokenMap[startToken], repsCount, currentAddres, IsLoop(startToken))
                );
                currentAddres++;
            }
            _parsedInstructions = grouped;
            return this;
        }

        public void Execute()
        {
            const int MEMORY_SIZE = 30_000;
            var memory = new byte[30_000];
            var head = 0;
            var ip = 0;
            while (ip < _parsedInstructions.Count)
            {
                var instruction = _parsedInstructions[ip];
                switch (instruction.Token)
                {
                    case TokenKind.Inc:
                        for (var j = 0; j < instruction.Count; j++)
                            memory[head] = (byte)((memory[head] + 1) % 256);
                        ip++;
                        break;
                    case TokenKind.Dec:
                        for (var j = 0; j < instruction.Count; j++)
                            memory[head] = (byte)((memory[head] - 1 + byte.MaxValue + 1) % 256);
                        ip++;
                        break;
                    case TokenKind.MoveLeft:
                        head = (head - instruction.Count + MEMORY_SIZE) % MEMORY_SIZE;
                        ip++;
                        break;
                    case TokenKind.MoveRight:
                        head = (head + instruction.Count) % MEMORY_SIZE;
                        ip++;
                        break;
                    case TokenKind.Print:
                        for (var j = 0; j < instruction.Count; j++)
                            Console.Write(
                                char.IsAscii((char)memory[head]) ? (char)memory[head] : memory[head]
                            );
                        ip++;
                        break;
                    case TokenKind.Read:
                        for (var j = 0; j < instruction.Count; j++)
                        {
                            var input = Console.Read();
                            memory[head] = input != -1 ? (byte)input : (byte)0;
                        }
                        ip++;
                        break;
                    case TokenKind.LoopStart:
                        ip = memory[head] == 0 ? instruction.InstructionAddress : ip + 1;
                        break;
                    case TokenKind.LoopEnd:
                        ip = memory[head] != 0 ? instruction.InstructionAddress : ip + 1;
                        break;
                    default:
                        throw new Exception("UNREACHABLE");
                }
            }
        }

        private static bool IsValidToken(char ch) => VALID_TOKENS.Contains(ch);

        private static bool IsLoop(char ch) => ch == '[' || ch == ']';

        private bool NextToken(out char ch)
        {
            if (_cursor >= _sourceCode.Length)
            {
                ch = (char)0;
                return false;
            }
            while (!IsValidToken(_sourceCode[_cursor]) && _cursor < _sourceCode.Length)
            {
                _cursor++;
            }

            ch = _sourceCode[_cursor];
            _cursor++;
            return true;
        }
    }
}
