Stack	res 10000	% The stack
fz	dw 16
	dw 16
cs	dw 4
	dw 98
	dw 99
	dw 122
	dw 110
gb	res 4
	entry	% Start here
	add r1,r0,r0	% Initialize top stack to 0
	add r10,r0,r0	% Initialize current frame size to zero
	jl r15,L0
	hlt 
L0	
		% TopSt, +, TopSt, (f, 0)
	add r3,r0,r1	% Put the address of the top of the stack in r3
	addi r4,r0,0
	lw r4,fz(r4)
	add r3,r3,r4
	add r1,r0,r3
		% (l, 0), =, RetAdd
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% (l, 4), -, TopSt, (f, 0)
	add r3,r0,r1	% Put the address of the top of the stack in r3
	addi r4,r0,0
	lw r4,fz(r4)
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% (l, 8), =, (f, 0)
	addi r3,r0,0
	lw r3,fz(r3)
	addi r13,r1,8
	sw 0(r13),r3
	jl r15,L1	% call L1, 1
		% (l, 12), =, Result
	add r3,r0,r14	% Put the returned value into r3
	addi r13,r1,12
	sw 0(r13),r3
		% write (l, 12)
	addi r3,r1,12
	lw r3,0(r3)
	putc r3
		% write (c, 4)
	addi r3,r0,4
	lw r3,cs(r3)
	putc r3
		% write (c, 8)
	addi r3,r0,8
	lw r3,cs(r3)
	putc r3
		% write (g, 0)
	addi r3,r0,0
	lw r3,gb(r3)
	putc r3
L2	
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
		% TopSt, +, TopSt, (f, 4)
	add r3,r0,r1	% Put the address of the top of the stack in r3
	addi r4,r0,4
	lw r4,fz(r4)
	add r3,r3,r4
	add r1,r0,r3
		% (l, 0), =, RetAdd
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% (l, 4), -, TopSt, (f, 4)
	add r3,r0,r1	% Put the address of the top of the stack in r3
	addi r4,r0,4
	lw r4,fz(r4)
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% (l, 8), =, (f, 4)
	addi r3,r0,4
	lw r3,fz(r3)
	addi r13,r1,8
	sw 0(r13),r3
		% (g, 0), =, (c, 12)
	addi r3,r0,12
	lw r3,cs(r3)
	addi r13,r0,0
	sw gb(r13),r3
		% Result, =, (c, 16)
	addi r3,r0,16
	lw r3,cs(r3)
	add r14,r0,r3
	j L3	% goto L3
L3	
		% RetAdd, =, (l, 0)
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% TopSt, =, (l, 4)
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% return
