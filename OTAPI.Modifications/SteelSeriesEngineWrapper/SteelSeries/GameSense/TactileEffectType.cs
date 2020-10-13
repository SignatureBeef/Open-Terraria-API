using System;

namespace SteelSeries.GameSense
{
	// Token: 0x02000067 RID: 103
	public enum TactileEffectType
	{
		// Token: 0x0400018E RID: 398
		ti_predefined_strongclick_100,
		// Token: 0x0400018F RID: 399
		ti_predefined_strongclick_60,
		// Token: 0x04000190 RID: 400
		ti_predefined_strongclick_30,
		// Token: 0x04000191 RID: 401
		ti_predefined_sharpclick_100,
		// Token: 0x04000192 RID: 402
		ti_predefined_sharpclick_60,
		// Token: 0x04000193 RID: 403
		ti_predefined_sharpclick_30,
		// Token: 0x04000194 RID: 404
		ti_predefined_softbump_100,
		// Token: 0x04000195 RID: 405
		ti_predefined_softbump_60,
		// Token: 0x04000196 RID: 406
		ti_predefined_softbump_30,
		// Token: 0x04000197 RID: 407
		ti_predefined_doubleclick_100,
		// Token: 0x04000198 RID: 408
		ti_predefined_doubleclick_60,
		// Token: 0x04000199 RID: 409
		ti_predefined_tripleclick_100,
		// Token: 0x0400019A RID: 410
		ti_predefined_softfuzz_60,
		// Token: 0x0400019B RID: 411
		ti_predefined_strongbuzz_100,
		// Token: 0x0400019C RID: 412
		ti_predefined_buzzalert750ms,
		// Token: 0x0400019D RID: 413
		ti_predefined_buzzalert1000ms,
		// Token: 0x0400019E RID: 414
		ti_predefined_strongclick1_100,
		// Token: 0x0400019F RID: 415
		ti_predefined_strongclick2_80,
		// Token: 0x040001A0 RID: 416
		ti_predefined_strongclick3_60,
		// Token: 0x040001A1 RID: 417
		ti_predefined_strongclick4_30,
		// Token: 0x040001A2 RID: 418
		ti_predefined_mediumclick1_100,
		// Token: 0x040001A3 RID: 419
		ti_predefined_mediumclick2_80,
		// Token: 0x040001A4 RID: 420
		ti_predefined_mediumclick3_60,
		// Token: 0x040001A5 RID: 421
		ti_predefined_sharptick1_100,
		// Token: 0x040001A6 RID: 422
		ti_predefined_sharptick2_80,
		// Token: 0x040001A7 RID: 423
		ti_predefined_sharptick3_60,
		// Token: 0x040001A8 RID: 424
		ti_predefined_shortdoubleclickstrong1_100,
		// Token: 0x040001A9 RID: 425
		ti_predefined_shortdoubleclickstrong2_80,
		// Token: 0x040001AA RID: 426
		ti_predefined_shortdoubleclickstrong3_60,
		// Token: 0x040001AB RID: 427
		ti_predefined_shortdoubleclickstrong4_30,
		// Token: 0x040001AC RID: 428
		ti_predefined_shortdoubleclickmedium1_100,
		// Token: 0x040001AD RID: 429
		ti_predefined_shortdoubleclickmedium2_80,
		// Token: 0x040001AE RID: 430
		ti_predefined_shortdoubleclickmedium3_60,
		// Token: 0x040001AF RID: 431
		ti_predefined_shortdoublesharptick1_100,
		// Token: 0x040001B0 RID: 432
		ti_predefined_shortdoublesharptick2_80,
		// Token: 0x040001B1 RID: 433
		ti_predefined_shortdoublesharptick3_60,
		// Token: 0x040001B2 RID: 434
		ti_predefined_longdoublesharpclickstrong1_100,
		// Token: 0x040001B3 RID: 435
		ti_predefined_longdoublesharpclickstrong2_80,
		// Token: 0x040001B4 RID: 436
		ti_predefined_longdoublesharpclickstrong3_60,
		// Token: 0x040001B5 RID: 437
		ti_predefined_longdoublesharpclickstrong4_30,
		// Token: 0x040001B6 RID: 438
		ti_predefined_longdoublesharpclickmedium1_100,
		// Token: 0x040001B7 RID: 439
		ti_predefined_longdoublesharpclickmedium2_80,
		// Token: 0x040001B8 RID: 440
		ti_predefined_longdoublesharpclickmedium3_60,
		// Token: 0x040001B9 RID: 441
		ti_predefined_longdoublesharptick1_100,
		// Token: 0x040001BA RID: 442
		ti_predefined_longdoublesharptick2_80,
		// Token: 0x040001BB RID: 443
		ti_predefined_longdoublesharptick3_60,
		// Token: 0x040001BC RID: 444
		ti_predefined_buzz1_100,
		// Token: 0x040001BD RID: 445
		ti_predefined_buzz2_80,
		// Token: 0x040001BE RID: 446
		ti_predefined_buzz3_60,
		// Token: 0x040001BF RID: 447
		ti_predefined_buzz4_40,
		// Token: 0x040001C0 RID: 448
		ti_predefined_buzz5_20,
		// Token: 0x040001C1 RID: 449
		ti_predefined_pulsingstrong1_100,
		// Token: 0x040001C2 RID: 450
		ti_predefined_pulsingstrong2_60,
		// Token: 0x040001C3 RID: 451
		ti_predefined_pulsingmedium1_100,
		// Token: 0x040001C4 RID: 452
		ti_predefined_pulsingmedium2_60,
		// Token: 0x040001C5 RID: 453
		ti_predefined_pulsingsharp1_100,
		// Token: 0x040001C6 RID: 454
		ti_predefined_pulsingsharp2_60,
		// Token: 0x040001C7 RID: 455
		ti_predefined_transitionclick1_100,
		// Token: 0x040001C8 RID: 456
		ti_predefined_transitionclick2_80,
		// Token: 0x040001C9 RID: 457
		ti_predefined_transitionclick3_60,
		// Token: 0x040001CA RID: 458
		ti_predefined_transitionclick4_40,
		// Token: 0x040001CB RID: 459
		ti_predefined_transitionclick5_20,
		// Token: 0x040001CC RID: 460
		ti_predefined_transitionclick6_10,
		// Token: 0x040001CD RID: 461
		ti_predefined_transitionhum1_100,
		// Token: 0x040001CE RID: 462
		ti_predefined_transitionhum2_80,
		// Token: 0x040001CF RID: 463
		ti_predefined_transitionhum3_60,
		// Token: 0x040001D0 RID: 464
		ti_predefined_transitionhum4_40,
		// Token: 0x040001D1 RID: 465
		ti_predefined_transitionhum5_20,
		// Token: 0x040001D2 RID: 466
		ti_predefined_transitionhum6_10,
		// Token: 0x040001D3 RID: 467
		ti_predefined_transitionrampdownlongsmooth1_100to0,
		// Token: 0x040001D4 RID: 468
		ti_predefined_transitionrampdownlongsmooth2_100to0,
		// Token: 0x040001D5 RID: 469
		ti_predefined_transitionrampdownmediumsmooth1_100to0,
		// Token: 0x040001D6 RID: 470
		ti_predefined_transitionrampdownmediumsmooth2_100to0,
		// Token: 0x040001D7 RID: 471
		ti_predefined_transitionrampdownshortsmooth1_100to0,
		// Token: 0x040001D8 RID: 472
		ti_predefined_transitionrampdownshortsmooth2_100to0,
		// Token: 0x040001D9 RID: 473
		ti_predefined_transitionrampdownlongsharp1_100to0,
		// Token: 0x040001DA RID: 474
		ti_predefined_transitionrampdownlongsharp2_100to0,
		// Token: 0x040001DB RID: 475
		ti_predefined_transitionrampdownmediumsharp1_100to0,
		// Token: 0x040001DC RID: 476
		ti_predefined_transitionrampdownmediumsharp2_100to0,
		// Token: 0x040001DD RID: 477
		ti_predefined_transitionrampdownshortsharp1_100to0,
		// Token: 0x040001DE RID: 478
		ti_predefined_transitionrampdownshortsharp2_100to0,
		// Token: 0x040001DF RID: 479
		ti_predefined_transitionrampuplongsmooth1_0to100,
		// Token: 0x040001E0 RID: 480
		ti_predefined_transitionrampuplongsmooth2_0to100,
		// Token: 0x040001E1 RID: 481
		ti_predefined_transitionrampupmediumsmooth1_0to100,
		// Token: 0x040001E2 RID: 482
		ti_predefined_transitionrampupmediumsmooth2_0to100,
		// Token: 0x040001E3 RID: 483
		ti_predefined_transitionrampupshortsmooth1_0to100,
		// Token: 0x040001E4 RID: 484
		ti_predefined_transitionrampupshortsmooth2_0to100,
		// Token: 0x040001E5 RID: 485
		ti_predefined_transitionrampuplongsharp1_0to100,
		// Token: 0x040001E6 RID: 486
		ti_predefined_transitionrampuplongsharp2_0to100,
		// Token: 0x040001E7 RID: 487
		ti_predefined_transitionrampupmediumsharp1_0to100,
		// Token: 0x040001E8 RID: 488
		ti_predefined_transitionrampupmediumsharp2_0to100,
		// Token: 0x040001E9 RID: 489
		ti_predefined_transitionrampupshortsharp1_0to100,
		// Token: 0x040001EA RID: 490
		ti_predefined_transitionrampupshortsharp2_0to100,
		// Token: 0x040001EB RID: 491
		ti_predefined_transitionrampdownlongsmooth1_50to0,
		// Token: 0x040001EC RID: 492
		ti_predefined_transitionrampdownlongsmooth2_50to0,
		// Token: 0x040001ED RID: 493
		ti_predefined_transitionrampdownmediumsmooth1_50to0,
		// Token: 0x040001EE RID: 494
		ti_predefined_transitionrampdownmediumsmooth2_50to0,
		// Token: 0x040001EF RID: 495
		ti_predefined_transitionrampdownshortsmooth1_50to0,
		// Token: 0x040001F0 RID: 496
		ti_predefined_transitionrampdownshortsmooth2_50to0,
		// Token: 0x040001F1 RID: 497
		ti_predefined_transitionrampdownlongsharp1_50to0,
		// Token: 0x040001F2 RID: 498
		ti_predefined_transitionrampdownlongsharp2_50to0,
		// Token: 0x040001F3 RID: 499
		ti_predefined_transitionrampdownmediumsharp1_50to0,
		// Token: 0x040001F4 RID: 500
		ti_predefined_transitionrampdownmediumsharp2_50to0,
		// Token: 0x040001F5 RID: 501
		ti_predefined_transitionrampdownshortsharp1_50to0,
		// Token: 0x040001F6 RID: 502
		ti_predefined_transitionrampdownshortsharp2_50to0,
		// Token: 0x040001F7 RID: 503
		ti_predefined_transitionrampuplongsmooth1_0to50,
		// Token: 0x040001F8 RID: 504
		ti_predefined_transitionrampuplongsmooth2_0to50,
		// Token: 0x040001F9 RID: 505
		ti_predefined_transitionrampupmediumsmooth1_0to50,
		// Token: 0x040001FA RID: 506
		ti_predefined_transitionrampupmediumsmooth2_0to50,
		// Token: 0x040001FB RID: 507
		ti_predefined_transitionrampupshortsmooth1_0to50,
		// Token: 0x040001FC RID: 508
		ti_predefined_transitionrampupshortsmooth2_0to50,
		// Token: 0x040001FD RID: 509
		ti_predefined_transitionrampuplongsharp1_0to50,
		// Token: 0x040001FE RID: 510
		ti_predefined_transitionrampuplongsharp2_0to50,
		// Token: 0x040001FF RID: 511
		ti_predefined_transitionrampupmediumsharp1_0to50,
		// Token: 0x04000200 RID: 512
		ti_predefined_transitionrampupmediumsharp2_0to50,
		// Token: 0x04000201 RID: 513
		ti_predefined_transitionrampupshortsharp1_0to50,
		// Token: 0x04000202 RID: 514
		ti_predefined_transitionrampupshortsharp2_0to50,
		// Token: 0x04000203 RID: 515
		ti_predefined_longbuzzforprogrammaticstopping_100,
		// Token: 0x04000204 RID: 516
		ti_predefined_smoothhum1nokickorbrakepulse_50,
		// Token: 0x04000205 RID: 517
		ti_predefined_smoothhum2nokickorbrakepulse_40,
		// Token: 0x04000206 RID: 518
		ti_predefined_smoothhum3nokickorbrakepulse_30,
		// Token: 0x04000207 RID: 519
		ti_predefined_smoothhum4nokickorbrakepulse_20,
		// Token: 0x04000208 RID: 520
		ti_predefined_smoothhum5nokickorbrakepulse_10
	}
}
