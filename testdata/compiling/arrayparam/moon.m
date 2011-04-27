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
		% (l, 416), =, (c, 8)
	addi r3,r0,8
	lw r3,cs(r3)
	addi r13,r1,416
	sw 0(r13),r3
		% param (l, 12)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,12
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% param (l, 416)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,416
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% call L3, 2
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L3
		% param (l, 12)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,12
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% param (l, 416)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,416
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% call L4, 2
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
		% (l, 424), =, Result
	add r3,r0,r14	% Put the returned value into r3
	addi r13,r1,424
	sw 0(r13),r3
		% (l, 420), ==, (l, 424), (c, 12)
	addi r3,r1,424
	lw r3,0(r3)
	addi r4,r0,12
	lw r4,cs(r4)
	ceq r3,r3,r4
	addi r13,r1,420
	sw 0(r13),r3
		% if_false (l, 420) goto L6
	addi r3,r1,420
	lw r3,0(r3)
	bz r3,L6
		% write (c, 16)
	addi r3,r0,16
	lw r3,cs(r3)
	putc r3
		% write (c, 20)
	addi r3,r0,20
	lw r3,cs(r3)
	putc r3
		% write (c, 24)
	addi r3,r0,24
	lw r3,cs(r3)
	putc r3
		% write (c, 28)
	addi r3,r0,28
	lw r3,cs(r3)
	putc r3
		% write (c, 32)
	addi r3,r0,32
	lw r3,cs(r3)
	putc r3
	j L7	% goto L7
L6	
		% write (c, 36)
	addi r3,r0,36
	lw r3,cs(r3)
	putc r3
		% write (c, 40)
	addi r3,r0,40
	lw r3,cs(r3)
	putc r3
		% write (c, 44)
	addi r3,r0,44
	lw r3,cs(r3)
	putc r3
		% write (c, 48)
	addi r3,r0,48
	lw r3,cs(r3)
	putc r3
		% write (c, 52)
	addi r3,r0,52
	lw r3,cs(r3)
	putc r3
L7	
L5	
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
		% (l, 16), ==, (p, 12), (c, 56)
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,56
	lw r4,cs(r4)
	ceq r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% if_false (l, 16) goto L9
	addi r3,r1,16
	lw r3,0(r3)
	bz r3,L9
		% write (c, 60)
	addi r3,r0,60
	lw r3,cs(r3)
	putc r3
	j L10	% goto L10
L9	
		% param (p, 12)
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,12
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% call L2, 1
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L2
L10	
		% Result, =, (c, 64)
	addi r3,r0,64
	lw r3,cs(r3)
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
		% (l, 24), !=, (p, 12), (c, 68)
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,68
	lw r4,cs(r4)
	cne r3,r3,r4
	addi r13,r1,24
	sw 0(r13),r3
		% if_false (l, 24) goto L12
	addi r3,r1,24
	lw r3,0(r3)
	bz r3,L12
		% (l, 16), %, (p, 12), (c, 72)
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,72
	lw r4,cs(r4)
	mod r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% (l, 20), /, (p, 12), (c, 76)
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,76
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
		% call L2, 1
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L2
		% (l, 16), +, (l, 16), (c, 80)
	addi r3,r1,16
	lw r3,0(r3)
	addi r4,r0,80
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% write (l, 16)
	addi r3,r1,16
	lw r3,0(r3)
	putc r3
	j L13	% goto L13
L12	
L13	
		% Result, =, (c, 84)
	addi r3,r0,84
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
		% (l, 20), =, (c, 88)
	addi r3,r0,88
	lw r3,cs(r3)
	addi r13,r1,20
	sw 0(r13),r3
L15	
		% (l, 24), <, (l, 20), (p, 16)
	addi r3,r1,20
	lw r3,0(r3)
	addi r4,r1,16
	lw r4,0(r4)
	clt r3,r3,r4
	addi r13,r1,24
	sw 0(r13),r3
		% if_false (l, 24) goto L16
	addi r3,r1,24
	lw r3,0(r3)
	bz r3,L16
		% (l, 12), [, (l, 20), (l, 20)
	addi r4,r1,12
	lw r4,0(r4)
	addi r3,r1,20
	lw r3,0(r3)
	sl r3,2	% Get the offset by multiplying by the word size
	add r3,r3,r4	% Add the address of the array to the offset
	addi r4,r1,20
	lw r4,0(r4)
	sw 0(r3),r4	% Set value in array
		% (l, 20), +, (l, 20), (c, 92)
	addi r3,r1,20
	lw r3,0(r3)
	addi r4,r0,92
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
	j L15	% goto L15
L16	
		% Result, =, (c, 96)
	addi r3,r0,96
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
		% (l, 20), =, (c, 100)
	addi r3,r0,100
	lw r3,cs(r3)
	addi r13,r1,20
	sw 0(r13),r3
		% (l, 28), =, (c, 104)
	addi r3,r0,104
	lw r3,cs(r3)
	addi r13,r1,28
	sw 0(r13),r3
L18	
		% (l, 32), <, (l, 20), (p, 16)
	addi r3,r1,20
	lw r3,0(r3)
	addi r4,r1,16
	lw r4,0(r4)
	clt r3,r3,r4
	addi r13,r1,32
	sw 0(r13),r3
		% if_false (l, 32) goto L19
	addi r3,r1,32
	lw r3,0(r3)
	bz r3,L19
		% (l, 36), (p, 12), [, (l, 20)
	addi r4,r1,12
	lw r4,0(r4)
	addi r3,r1,20
	lw r3,0(r3)
	sl r3,2	% Get the offset by multiplying by the word size
	add r3,r3,r4	% Add the address of the array to the offset
	lw r3,0(r3)	% Read the value at the index
	addi r13,r1,36
	sw 0(r13),r3
		% (l, 32), !=, (l, 36), (l, 20)
	addi r3,r1,36
	lw r3,0(r3)
	addi r4,r1,20
	lw r4,0(r4)
	cne r3,r3,r4
	addi r13,r1,32
	sw 0(r13),r3
		% if_false (l, 32) goto L20
	addi r3,r1,32
	lw r3,0(r3)
	bz r3,L20
		% (l, 28), =, (c, 108)
	addi r3,r0,108
	lw r3,cs(r3)
	addi r13,r1,28
	sw 0(r13),r3
	j L21	% goto L21
L20	
L21	
		% (l, 20), +, (l, 20), (c, 112)
	addi r3,r1,20
	lw r3,0(r3)
	addi r4,r0,112
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
	j L18	% goto L18
L19	
		% write (c, 116)
	addi r3,r0,116
	lw r3,cs(r3)
	putc r3
		% Result, =, (l, 28)
	addi r3,r1,28
	lw r3,0(r3)
	add r14,r0,r3
	j L17	% goto L17
L17	
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
fz	dw 428
	dw 24
	dw 32
	dw 32
	dw 40
cs	dw 0
	dw 16
	dw 100
	dw 1
	dw 80
	dw 65
	dw 83
	dw 83
	dw 10
	dw 70
	dw 65
	dw 73
	dw 76
	dw 10
	dw 0
	dw 48
	dw 0
	dw 0
	dw 10
	dw 10
	dw 48
	dw 0
	dw 0
	dw 1
	dw 0
	dw 0
	dw 1
	dw 0
	dw 1
	dw 10
gb	res 0
st	res 10000	% The stack
