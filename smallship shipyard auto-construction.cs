
IMyMotorAdvancedStator ShipyardAdRotor;
IMyProgrammableBlock PgBlock;
IMyCubeGrid ThisGrid,ProjectedGrid;
IMyProjector ShipyardProjector;
IMyShipWelder[] ShipyardWelderset = new IMyShipWelder[5];
IMyPistonBase[] WelderPistonset = new IMyPistonBase[5];
IMyPistonBase YardPiston1, YardPiston2;
IMyShipMergeBlock Docker;
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    for(int i=0;i<4;i++)
    {
      ShipyardWelderset[i] = GridTerminalSystem.GetBlockWithName("ShipyardWelder"+(i+1)) as IMyShipWelder;
      WelderPistonset[i] = GridTerminalSystem.GetBlockWithName("Welder"+(i+1)+"Piston") as IMyPistonBase;
    }
    Docker = GridTerminalSystem.GetBlockWithName("ShipDocker") as IMyShipMergeBlock;
    YardPiston1 = GridTerminalSystem.GetBlockWithName("ShipyardPiston1") as IMyPistonBase;
    YardPiston2 = GridTerminalSystem.GetBlockWithName("ShipyardPiston2") as IMyPistonBase;
    ShipyardProjector = GridTerminalSystem.GetBlockWithName("ShipyardProjector") as IMyProjector;
    ShipyardAdRotor = GridTerminalSystem.GetBlockWithName("ShipyardAdRotor") as IMyMotorAdvancedStator;
    PgBlock = GridTerminalSystem.GetBlockWithName("test") as IMyProgrammableBlock;
    ThisGrid = PgBlock.CubeGrid;
    ProjectedGrid = ShipyardProjector.CubeGrid;
    if(PgBlock.Enabled == true)
      ConstructInit();
    else
      TerminateSystem();
}
void TerminateSystem()
{
    Docker.ApplyAction("OnOff_Off");
    YardPiston1.ApplyAction("Retract");
    YardPiston2.ApplyAction("Retract");
    ShipyardAdRotor.RotorLock = true;
    for(int i=0;i<4;i++)
    {
        ShipyardWelderset[i].ApplyAction("OnOff_Off");
        WelderPistonset[i].ApplyAction("Retract");
    }
}
void ConstructInit()
{
    Docker.ApplyAction("OnOff_On");
    YardPiston1.ApplyAction("Retract");
    YardPiston2.ApplyAction("Retract");
    ShipyardAdRotor.RotorLock = false;
    for(int i=0;i<4;i++)
    {
        ShipyardWelderset[i].ApplyAction("OnOff_On");
        WelderPistonset[i].ApplyAction("Extend");
    }
}
public void Main(string argument, UpdateType updateSource)
{
    List<IMyTerminalBlock> m_blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(m_blocks);

    for(int welder=0;welder<4;welder++)
    {
      double minDist = float.MaxValue;
      foreach(var i in m_blocks)
      {
        if(i.CubeGrid == ProjectedGrid)
        {
          double dist = calcDist(i.GetPosition(), ShipyardWelderset[welder].GetPosition());
          if(minDist > dist)
            minDist = dist;
        }
      }
      Echo("index "+welder+" minDist: "+minDist.ToString());
      if(minDist < 3.5)
      {
        WelderPistonset[welder].ApplyAction("Retract");
      }
      if(minDist > 4.0)
      {
        WelderPistonset[welder].ApplyAction("Extend");
      }
    }
    if(ShipyardProjector.RemainingBlocks == 0)
    {
      TerminateSystem();
      PgBlock.ApplyAction("OnOff_Off");
    }

}
double calcDist(Vector3D pos1, Vector3D pos2)
{
  return (pos1 - pos2).Length();
}
