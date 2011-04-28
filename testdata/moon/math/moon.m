	% Top of stack: r1
	% Global vars: r2
	% Accumulator 1: r3
	% Accumulator 2: r4
	% Current Frame size: r10
	% Parameter passing: r11
	% helper: r13
	% Return value: r14
	% Return address: r15
	%  
	% start
	entry 
	addi r1,r0,st	% Initialize top stack address
	% FrSz, =, (c, 0)
	addi r3,r0,0
	lw r3,cs(r3)
	add r10,r0,r3
	% call L0, 0
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L0
	% halt
	hlt 
L0
	% TopSt, +, TopSt, FrSz
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
	% (l, 0), =, RetAdd
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
	% (l, 4), -, TopSt, FrSz
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
	% (l, 8), =, FrSz
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
	% FrSz, =, (f, 0)
	addi r3,r0,0
	lw r3,fz(r3)
	add r10,r0,r3
	% param (c, 4)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,4
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
	% call L1, 1
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L1
	% param (c, 8)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,8
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
	% call L2, 1
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L2
	% param (c, 12)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,12
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
	% call L3, 1
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L3
L7
	% FrSz, =, (l, 8)
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
	% RetAdd, =, (l, 0)
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
	% TopSt, =, (l, 4)
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% return
L1
	% TopSt, +, TopSt, FrSz
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
	% (l, 0), =, RetAdd
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
	% (l, 4), -, TopSt, FrSz
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
	% (l, 8), =, FrSz
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
	% FrSz, =, (f, 4)
	addi r3,r0,4
	lw r3,fz(r3)
	add r10,r0,r3
	% (l, 16), /, (c, 20), (c, 24)
	addi r3,r0,20
	lw r3,cs(r3)
	addi r4,r0,24
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
	% (l, 16), +, (c, 16), (l, 16)
	addi r3,r0,16
	lw r3,cs(r3)
	addi r4,r1,16
	lw r4,0(r4)
	add r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
	% param (l, 16)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
	% param (c, 28)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,28
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
	% call L4, 2
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
	% Result, =, Result
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L8	% goto L8
L8
	% FrSz, =, (l, 8)
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
	% RetAdd, =, (l, 0)
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
	% TopSt, =, (l, 4)
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% return
L2
	% TopSt, +, TopSt, FrSz
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
	% (l, 0), =, RetAdd
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
	% (l, 4), -, TopSt, FrSz
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
	% (l, 8), =, FrSz
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
	% FrSz, =, (f, 8)
	addi r3,r0,8
	lw r3,fz(r3)
	add r10,r0,r3
	% (l, 24), +, (c, 32), (c, 36)
	addi r3,r0,32
	lw r3,cs(r3)
	addi r4,r0,36
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,24
	sw 0(r13),r3
	% (l, 16), /, (l, 24), (c, 40)
	addi r3,r1,24
	lw r3,0(r3)
	addi r4,r0,40
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
	% param (l, 16)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
	% param (c, 44)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,44
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
	% call L4, 2
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
	% Result, =, Result
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L9	% goto L9
L9
	% FrSz, =, (l, 8)
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
	% RetAdd, =, (l, 0)
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
	% TopSt, =, (l, 4)
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% return
L3
	% TopSt, +, TopSt, FrSz
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
	% (l, 0), =, RetAdd
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
	% (l, 4), -, TopSt, FrSz
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
	% (l, 8), =, FrSz
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
	% FrSz, =, (f, 12)
	addi r3,r0,12
	lw r3,fz(r3)
	add r10,r0,r3
	% (l, 16), -, (c, 52), (c, 48)
	addi r3,r0,52
	lw r3,cs(r3)
	addi r4,r0,48
	lw r4,cs(r4)
	sub r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
	% (l, 20), -, (c, 56), (c, 60)
	addi r3,r0,56
	lw r3,cs(r3)
	addi r4,r0,60
	lw r4,cs(r4)
	sub r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
	% param (l, 16)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
	% param (l, 20)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,20
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
	% call L4, 2
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
	% Result, =, Result
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L10	% goto L10
L10
	% FrSz, =, (l, 8)
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
	% RetAdd, =, (l, 0)
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
	% TopSt, =, (l, 4)
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% return
L4
	% TopSt, +, TopSt, FrSz
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
	% (l, 0), =, RetAdd
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
	% (l, 4), -, TopSt, FrSz
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
	% (l, 8), =, FrSz
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
	% FrSz, =, (f, 16)
	addi r3,r0,16
	lw r3,fz(r3)
	add r10,r0,r3
	% (l, 20), ==, (p, 12), (p, 16)
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r1,16
	lw r4,0(r4)
	ceq r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
	% if_false (l, 20) goto L12
	addi r3,r1,20
	lw r3,0(r3)
	bz r3,L12
	% write (c, 64)
	addi r3,r0,64
	lw r3,cs(r3)
	putc r3
	% write (c, 68)
	addi r3,r0,68
	lw r3,cs(r3)
	putc r3
	% write (c, 72)
	addi r3,r0,72
	lw r3,cs(r3)
	putc r3
	% write (c, 76)
	addi r3,r0,76
	lw r3,cs(r3)
	putc r3
	% write (c, 80)
	addi r3,r0,80
	lw r3,cs(r3)
	putc r3
	j L13	% goto L13
L12
	% write (c, 84)
	addi r3,r0,84
	lw r3,cs(r3)
	putc r3
	% write (c, 88)
	addi r3,r0,88
	lw r3,cs(r3)
	putc r3
	% write (c, 92)
	addi r3,r0,92
	lw r3,cs(r3)
	putc r3
	% write (c, 96)
	addi r3,r0,96
	lw r3,cs(r3)
	putc r3
	% write (c, 100)
	addi r3,r0,100
	lw r3,cs(r3)
	putc r3
	% write (c, 104)
	addi r3,r0,104
	lw r3,cs(r3)
	putc r3
	% write (c, 108)
	addi r3,r0,108
	lw r3,cs(r3)
	putc r3
	% param (p, 12)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,12
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
	% call L5, 1
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L5
	% write (c, 112)
	addi r3,r0,112
	lw r3,cs(r3)
	putc r3
	% write (c, 116)
	addi r3,r0,116
	lw r3,cs(r3)
	putc r3
	% write (c, 120)
	addi r3,r0,120
	lw r3,cs(r3)
	putc r3
	% param (p, 16)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
	% call L5, 1
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L5
	% write (c, 124)
	addi r3,r0,124
	lw r3,cs(r3)
	putc r3
L13
	% Result, =, (c, 128)
	addi r3,r0,128
	lw r3,cs(r3)
	add r14,r0,r3
	j L11	% goto L11
L11
	% FrSz, =, (l, 8)
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
	% RetAdd, =, (l, 0)
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
	% TopSt, =, (l, 4)
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% return
L5
	% TopSt, +, TopSt, FrSz
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
	% (l, 0), =, RetAdd
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
	% (l, 4), -, TopSt, FrSz
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
	% (l, 8), =, FrSz
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
	% FrSz, =, (f, 20)
	addi r3,r0,20
	lw r3,fz(r3)
	add r10,r0,r3
	% (l, 16), ==, (p, 12), (c, 132)
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,132
	lw r4,cs(r4)
	ceq r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
	% if_false (l, 16) goto L15
	addi r3,r1,16
	lw r3,0(r3)
	bz r3,L15
	% write (c, 136)
	addi r3,r0,136
	lw r3,cs(r3)
	putc r3
	j L16	% goto L16
L15
	% (l, 16), <, (p, 12), (c, 140)
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,140
	lw r4,cs(r4)
	clt r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
	% if_false (l, 16) goto L17
	addi r3,r1,16
	lw r3,0(r3)
	bz r3,L17
	% write (c, 144)
	addi r3,r0,144
	lw r3,cs(r3)
	putc r3
	% (p, 12), -, (c, 148), (p, 12)
	addi r3,r0,148
	lw r3,cs(r3)
	addi r4,r1,12
	lw r4,0(r4)
	sub r3,r3,r4
	addi r13,r1,12
	sw 0(r13),r3
	j L18	% goto L18
L17
L18
L16
	% param (p, 12)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,12
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
	% call L6, 1
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L6
	% Result, =, (c, 152)
	addi r3,r0,152
	lw r3,cs(r3)
	add r14,r0,r3
	j L14	% goto L14
L14
	% FrSz, =, (l, 8)
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
	% RetAdd, =, (l, 0)
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
	% TopSt, =, (l, 4)
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% return
L6
	% TopSt, +, TopSt, FrSz
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
	% (l, 0), =, RetAdd
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
	% (l, 4), -, TopSt, FrSz
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
	% (l, 8), =, FrSz
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
	% FrSz, =, (f, 24)
	addi r3,r0,24
	lw r3,fz(r3)
	add r10,r0,r3
	% (l, 24), !=, (p, 12), (c, 156)
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,156
	lw r4,cs(r4)
	cne r3,r3,r4
	addi r13,r1,24
	sw 0(r13),r3
	% if_false (l, 24) goto L20
	addi r3,r1,24
	lw r3,0(r3)
	bz r3,L20
	% (l, 16), %, (p, 12), (c, 160)
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,160
	lw r4,cs(r4)
	mod r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
	% (l, 20), /, (p, 12), (c, 164)
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,164
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
	% param (l, 20)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,20
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
	% call L6, 1
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L6
	% (l, 16), +, (l, 16), (c, 168)
	addi r3,r1,16
	lw r3,0(r3)
	addi r4,r0,168
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
	% write (l, 16)
	addi r3,r1,16
	lw r3,0(r3)
	putc r3
	j L21	% goto L21
L20
L21
	% Result, =, (c, 172)
	addi r3,r0,172
	lw r3,cs(r3)
	add r14,r0,r3
	j L19	% goto L19
L19
	% FrSz, =, (l, 8)
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
	% RetAdd, =, (l, 0)
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
	% TopSt, =, (l, 4)
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% return
fz	dw 12
	dw 32
	dw 28
	dw 28
	dw 28
	dw 24
	dw 32
cs	dw 0
	dw 0
	dw 0
	dw 0
	dw 3
	dw 10
	dw 10
	dw 4
	dw 3
	dw 10
	dw 10
	dw 1
	dw 10
	dw 0
	dw 0
	dw 10
	dw 80
	dw 65
	dw 83
	dw 83
	dw 10
	dw 70
	dw 65
	dw 73
	dw 76
	dw 45
	dw 62
	dw 32
	dw 32
	dw 58
	dw 32
	dw 10
	dw 0
	dw 0
	dw 48
	dw 0
	dw 45
	dw 0
	dw 0
	dw 0
	dw 10
	dw 10
	dw 48
	dw 0
gb	res 0
st	res 10000	% The stack
