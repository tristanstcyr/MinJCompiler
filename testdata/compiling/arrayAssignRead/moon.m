		% Top of stack: r1
		% Global vars: r2
		% Accumulator 1: r3
		% Accumulator 2: r4
		% Accumulator 3: r5
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
		% (l, 12), +, TopSt, (c, 4)
	add r3,r0,r1	% Put the address of the top of the stack in r3
	addi r4,r0,4
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,12
	sw 0(r13),r3
		% (l, 60), =, (c, 8)
	addi r3,r0,8
	lw r3,cs(r3)
	addi r13,r1,60
	sw 0(r13),r3
		% (l, 56), =, (c, 12)
	addi r3,r0,12
	lw r3,cs(r3)
	addi r13,r1,56
	sw 0(r13),r3
L2	
		% (l, 68), <, (l, 56), (l, 60)
	addi r3,r1,56
	lw r3,0(r3)
	addi r4,r1,60
	lw r4,0(r4)
	clt r3,r3,r4
	addi r13,r1,68
	sw 0(r13),r3
		% if_false (l, 68) goto L3
	addi r3,r1,68
	lw r3,0(r3)
	bz r3,L3
		% (l, 12), [, (l, 56), (l, 56)
	addi r4,r1,12
	lw r4,0(r4)
	addi r3,r1,56
	lw r3,0(r3)
	sl r3,2	% Get the offset by multiplying by the word size
	add r3,r3,r4	% Add the address of the array to the offset
	addi r4,r1,56
	lw r4,0(r4)
	sw 0(r3),r4	% Set value in array
		% (l, 56), +, (l, 56), (c, 16)
	addi r3,r1,56
	lw r3,0(r3)
	addi r4,r0,16
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,56
	sw 0(r13),r3
	j L2	% goto L2
L3	
		% (l, 56), =, (c, 20)
	addi r3,r0,20
	lw r3,cs(r3)
	addi r13,r1,56
	sw 0(r13),r3
L4	
		% (l, 68), <, (l, 56), (l, 60)
	addi r3,r1,56
	lw r3,0(r3)
	addi r4,r1,60
	lw r4,0(r4)
	clt r3,r3,r4
	addi r13,r1,68
	sw 0(r13),r3
		% if_false (l, 68) goto L5
	addi r3,r1,68
	lw r3,0(r3)
	bz r3,L5
		% (l, 72), (l, 12), [, (l, 56)
	addi r4,r1,12
	lw r4,0(r4)
	addi r3,r1,56
	lw r3,0(r3)
	sl r3,2	% Get the offset by multiplying by the word size
	add r3,r3,r4	% Add the address of the array to the offset
	lw r3,0(r3)	% Read the value at the index
	addi r13,r1,72
	sw 0(r13),r3
		% (l, 64), +, (l, 72), (c, 24)
	addi r3,r1,72
	lw r3,0(r3)
	addi r4,r0,24
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,64
	sw 0(r13),r3
		% write (l, 64)
	addi r3,r1,64
	lw r3,0(r3)
	putc r3
		% (l, 56), +, (l, 56), (c, 28)
	addi r3,r1,56
	lw r3,0(r3)
	addi r4,r0,28
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,56
	sw 0(r13),r3
	j L4	% goto L4
L5	
L1	
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
fz	dw 76
cs	dw 0
	dw 16
	dw 10
	dw 0
	dw 1
	dw 0
	dw 48
	dw 1
gb	res 0
st	res 10000	% The stack
