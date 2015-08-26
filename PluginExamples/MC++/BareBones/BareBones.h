// BareBones.h

#pragma once

using namespace System;
using namespace tdsm::api::Command;
using namespace tdsm::api::Plugin;

namespace BareBones {

	public ref class Class1 : tdsm::api::BasePlugin
	{
		// TODO: Add your methods for this class here.
	public:
		Class1()
		{
			this->TDSMBuild = 1;
			this->Version = "1";
			this->Author = "TDSM";
			this->Name = "Simple name";
			this->Description = "This plugin does these awesome things!";
		}

	protected:
		//virtual void Initialized(Object state);

	private:
		//void MyCustomCommandCallback(ISender^ sender, ArgumentList^ args);

		//[Hook(HookOrder::NORMAL)]
		//void MyFunctionNameThatDoesntMatter(HookContext^ &ctx, HookArgs::PlayerEnteredGame^ args);
	};
}
