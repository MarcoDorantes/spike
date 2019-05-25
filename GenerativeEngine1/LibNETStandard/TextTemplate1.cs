


namespace Generated
{
	public class Derived : LibNETStandard.Class1
	{
		private SharedLibNETStandard.CrossClass1 cross;

		public Derived()
		{
			cross = new SharedLibNETStandard.CrossClass1();
		}

		public string Execute()
		{
			var result = new System.Text.StringBuilder();
			cross.Setup();
			result.AppendLine($"cross OK: {cross.OK}");
			result.AppendLine($"cross ExecutionSummary: {cross.GetExecutionSummary()}");
			return $"{result}";
		}
	}
//Generated summary: Additional code to be generated here for not OK case.
}