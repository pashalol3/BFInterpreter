using BFCompiler;

internal class Program
{
    private const string CALC_PI = """
        [ yet another pi calculation program in bf

         Just like for pi16.b the accuracy of the result depends on the cellsize:

          - using  8 bit cells causes an overflow after 4 digits
          - using 16 bit cells causes an overflow after 537 digits
          - using 32 bit cells causes an overflow after several millions of digits

         It's about ~38 times shorter than pi16.b, ~364 times faster and works with
         not-wrapping (bignum) implementations. 

         by Felix Nawothnig (felix.nawothnig@t-online.de) ]

           >  +++++ +++++ +++++ (15 digits)

           [<+>>>>>>>>++++++++++<<<<<<<-]>+++++[<+++++++++>-]+>>>>>>+[<<+++[>>[-<]<[>]<-]>>
           [>+>]<[<]>]>[[->>>>+<<<<]>>>+++>-]<[<<<<]<<<<<<<<+[->>>>>>>>>>>>[<+[->>>>+<<<<]>
           >>>>]<<<<[>>>>>[<<<<+>>>>-]<<<<<-[<<++++++++++>>-]>>>[<<[<+<<+>>>-]<[>+<-]<++<<+
           >>>>>>-]<<[-]<<-<[->>+<-[>>>]>[[<+>-]>+>>]<<<<<]>[-]>+<<<-[>>+<<-]<]<<<<+>>>>>>>
           >[-]>[<<<+>>>-]<<++++++++++<[->>+<-[>>>]>[[<+>-]>+>>]<<<<<]>[-]>+>[<<+<+>>>-]<<<
           <+<+>>[-[-[-[-[-[-[-[-[-<->[-<+<->>]]]]]]]]]]<[+++++[<<<++++++++<++++++++>>>>-]<
           <<<+<->>>>[>+<<<+++++++++<->>>-]<<<<<[>>+<<-]+<[->-<]>[>>.<<<<[+.[-]]>>-]>[>>.<<
        -]>[-]>[-]>>>[>>[<<<<<<<<+>>>>>>>>-]<<-]]>>[-]<<<[-]<<<<<<<<]++++++++++.
""";
    private const string TEST_PROGRAM = "+++.-.-.";

    private static void Main(string[] args)
    {
#if DEBUG
        new BFInterpreter(TEST_PROGRAM);
#endif
#if RELEASE
        if (args.Length == 0)
            LogError($"ERROR: no input file");
        if (args.Length > 1)
            LogError($"ERROR: too many input files");
        var filePath = args[0];
        if (!File.Exists(filePath))
            LogError("ERROR: invalid file path");
        var fileContnet = File.ReadAllText(filePath);
        new BFInterpreter(fileContnet);
#endif
    }

    private static void LogError(string err)
    {
        Console.WriteLine();
        Environment.Exit(1);
    }
}
