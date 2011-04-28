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
L2
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
	% Result, =, (c, 8)
	addi r3,r0,8
	lw r3,cs(r3)
	add r14,r0,r3
	j L3	% goto L3
L3
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
	dw 16
cs	dw 0
	dw 0
	dw 0
gb	res 0
st	res 10000	% The stack
