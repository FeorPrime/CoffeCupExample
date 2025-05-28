using System.ComponentModel.DataAnnotations;
using MassTransit;

namespace contracts.Entities;

public class TestStateMachineState: SagaStateMachineInstance
{
    // [Key]
    // public Guid Id {get; set;}
    [Key]
    public Guid CorrelationId { get; set; }
    
    public string CurrentState { get; set; }
    
    public Guid MachineId { get; set; }
    
    public string StockExchange { get; set; }
    
    //events
    public bool Step1Done { get; set; } = false;
    public bool Step2Done { get; set; } = false;
    //#events
    
    public DateTime? StartStamp { get; set; }
    public DateTime? EndStamp { get; set; }
    
    //public byte[] RowVersion { get; set; } = null!;
}

public class STMHelper
{
    public static void PrintSTMInfo(TestStateMachineState state)
    {
        PrintPretty($"ID:{state.CorrelationId} | Worker: {state.MachineId} | State: {state.CurrentState} | Step1Done: {state.Step1Done} | Step2Done: {state.Step2Done}");
    }

    public static void PrintPretty(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("------------------------");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("------------------------");
        Console.ResetColor();
    }
}