IMyShipConnector[] Docker = new IMyShipConnector[3];
IMyPistonBase[] Piston = new IMyPistonBase[3];
IMyProgrammableBlock M;
IMyTimerBlock TimerBlock;
public Program()
{
       for(int i=0;i<2;i++)
       {
               Docker[i] = GridTerminalSystem.GetBlockWithName("FactoryDocker"+(i+1)) as IMyShipConnector;
               Piston[i] = GridTerminalSystem.GetBlockWithName("FactoryDocker"+(i+1)+"Piston") as IMyPistonBase;
       }
       M = GridTerminalSystem.GetBlockWithName("FactoryDockerController") as IMyProgrammableBlock ;
       TimerBlock = GridTerminalSystem.GetBlockWithName("FactoryDockerTimer") as IMyTimerBlock;
}

public void Main(string argument, UpdateType updateSource)
{
       TimerBlock.ApplyAction("Start");
       Echo("TimerOn");
       for(int i=0;i<2;i++)
       {
               Echo("Docker "+i+" : "+Docker[i].Status.ToString());
               if(Docker[i].Status.ToString() == "Connected")
                       Piston[i].ApplyAction("Retract"); 
               else if(Docker[i].Status.ToString() == "Unconnected")
                       Piston[i].ApplyAction("Extend");
       }
}
