// This is the main DLL file.

#include "stdafx.h"

#include "BareBones.h"

using namespace System;
using namespace tdsm::api::Command;
using namespace Microsoft::Xna::Framework;
//
//void BareBones::Class1::Initialized(Object state)
//{
//	this->AddCommand("mcpp")
//		->WithAccessLevel(AccessLevel::PLAYER)
//		->WithDescription("My command description")
//		->WithHelpText("<name>")
//		->WithHelpText("<something else> <maybe more>")
//		->WithPermissionNode("BareBones.commandname")
//		->Calls(gcnew Action<ISender^, ArgumentList^>(this, &Class1::MyCustomCommandCallback));
//}

//
//void BareBones::Class1::MyCustomCommandCallback(ISender^ sender, ArgumentList^ args)
//{
//	//Your implementation
//	sender->SendMessage("Hi from managed C++", 255, 255, 0, 0);
//}
//
//void BareBones::Class1::MyFunctionNameThatDoesntMatter(HookContext^ &ctx, HookArgs::PlayerEnteredGame^ args)
//{
//	//Your implementation
//	ctx->Player->SendMessage("Welcome to the game from Managed C++", Color::Green);
//}